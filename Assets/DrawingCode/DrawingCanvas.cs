/* 
 * mayra barrera, 2017
 * class that reprenset a canvas (main class that receive event's)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DrawingCanvas : MonoBehaviour
{
    //control variables that specify user actions
    private bool drawing;

    //strokes
    private Dictionary<int, Stroke> allStrokes;
    private bool firstStroke;
    private int currentStrokeName;
    private int strokeCount;

    //classes that control user actions
    private StrokeCreationModule strokeCreation;
    private ExperimentControl experimentControl;

    #region initi

    public void Awake()
    {
        //event listeners, almost all of them come from the controllers
        EventManager.Instance.AddListener<StartDrawingEvent>(StartDrawing);
        EventManager.Instance.AddListener<EndDrawingEvent>(EndDrawing);
        EventManager.Instance.AddListener<OnDrawingEvent>(UpdatePosition);

        EventManager.Instance.AddListener<NewCanvasEvent>(DeleteEverything);
    }

    void Start()
    {
        InitiateGlobalVariables();

        allStrokes = new Dictionary<int, Stroke>();
        strokeCount = 0;

        strokeCreation = transform.GetComponent<StrokeCreationModule>();
        experimentControl = transform.GetComponent<ExperimentControl>();
    }

    private void InitiateGlobalVariables()
    {
        drawing = false;
        firstStroke = true;

        GlobalVars.Instance.currentStrokeColor = Color.black;
        GlobalVars.Instance.currentStrokeSize = 0.025f;
    }

    #endregion

    #region drawing
    void UpdatePosition(OnDrawingEvent e)
    {
        //current control position
        Transform control = e.transform;

        //STROKE
        #region Create Stroke
        if (drawing)
        {
            strokeCreation.onDrawing(control);
        }
        #endregion

    }

    void StartDrawing(StartDrawingEvent e)
    {
        Debug.Log("StartDrawing is called");
        //standard control variables
        drawing = true;

        //Log new stroke start (position object)
        experimentControl.StartStroke(e.brush.transform.position);

        //Create new stroke
        if (allStrokes.ContainsKey(strokeCount))
        {
            //sometimes I have an error about duplicated key, this is to avoid that error (NOT idea why the error happens)
            strokeCount++;
        }

        allStrokes.Add(strokeCount, new Stroke(strokeCount, GlobalVars.Instance.currentStrokeSize, GlobalVars.Instance.currentStrokeColor));
        strokeCreation.StartDrawing(allStrokes[strokeCount], e.brush.position);

        currentStrokeName = strokeCount;
    }

    void EndDrawing(EndDrawingEvent e)
    {
        Debug.Log("EndDrawing is called");
        drawing = false;
        Transform control = e.brush.transform;

        //Log new stroke end (position object)
        experimentControl.EndStroke(control.position);

        //Close new stroke      
        bool exist = strokeCreation.EndDrawing(control);

        if (!exist)
        {
            //stroke
            Destroy(allStrokes[strokeCount].stroke);
            allStrokes[strokeCount].DestroyStroke();
            allStrokes.Remove(strokeCount);
            strokeCount--;
        }

        if (firstStroke && exist)
        {

            Stroke stroke = allStrokes[0];

            Vector3 initPos = stroke.GetStrokePosition(0).position;
            Vector3 lastPos = stroke.GetStrokePosition(stroke.LastIndex() - 1).position;

            Vector3 direction = VectorMathsLibrary.DirectionBetweenPoints(lastPos, initPos);

            firstStroke = false;
        }

        //hide stroke (if we need to hide it for some reason!!!)
        //allStrokes[strokeCount].stroke.SetActive(false);

        //
        strokeCount++;
        currentStrokeName = -1;
    }
    #endregion

    #region menuActions

    //clean whole canvas
    private void DeleteEverything(NewCanvasEvent e)
    {
        Debug.Log("entre");

        //control log
        experimentControl.Phase(strokeCount);

        //strokes
        foreach (Stroke s in allStrokes.Values)
        {
            Destroy(s.stroke);
        }

        allStrokes.Clear();
        strokeCount = 0;

        //global
        InitiateGlobalVariables();
    }

    #endregion

}
