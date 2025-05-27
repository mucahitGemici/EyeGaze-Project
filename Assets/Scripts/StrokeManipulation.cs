using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
public class StrokeManipulation : MonoBehaviour
{
    private List<Transform> potentialStrokeSelectionList = new List<Transform>();

    private List<Transform> selectedStrokes = new List<Transform>();
    private List<Transform> selectedStrokesForScaling = new List<Transform>();
    private Transform selectedStrokesCommonParent;

    [SerializeField] private DrawController drawController;
    [SerializeField] private GazeReader gazeReader;
    [SerializeField] private PositionManipulator positionManipulator;
    [SerializeField] private ScaleManipulator scaleManipulator;

    [SerializeField] private Transform strokeHolder;


    private bool canManipulate = false;
    private void ClearSelectedStrokes()
    {
        foreach (Transform t in selectedStrokes)
        {
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque when selected strokes resetted
            t.GetComponent<MeshRenderer>().material.color = matColor;
            if (t.parent != strokeHolder) t.parent = strokeHolder;
        }

        selectedStrokes.Clear();
        selectedStrokesForScaling.Clear();
    }

    private void ClearPotentialStrokes()
    {
        foreach(Transform t in potentialStrokeSelectionList)
        {
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque when potential strokes resetted
            t.GetComponent<MeshRenderer>().material.color = matColor;
            if(t.parent != strokeHolder) t.parent = strokeHolder;
        }
        
        potentialStrokeSelectionList.Clear();
    }

    private void AssignSelectedStrokes()
    {
        List<Vector3> strokesPositionList = new List<Vector3>();
        List<GameObject> listForSettingCenter = new List<GameObject>();
        foreach (Transform t in potentialStrokeSelectionList)
        {
            Bounds meshBound = t.GetComponent<MeshCollider>().bounds;
            Vector3 center = meshBound.center;
            //GameObject test = Instantiate(testObject, center, Quaternion.identity);
            GameObject newPivot = new GameObject($"{t.name}_newPivot");
            newPivot.transform.position = center;
            strokesPositionList.Add(center);
            //test.transform.position = childObj.transform.position;
            t.parent = newPivot.transform;

            selectedStrokes.Add(t.transform);
            selectedStrokesForScaling.Add(newPivot.transform);
            listForSettingCenter.Add(newPivot);
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque after approving selection
            t.GetComponent<MeshRenderer>().material.color = matColor;
        }
        Vector3 sum = Vector3.zero;
        foreach(Vector3 vec in strokesPositionList)
        {
            sum += vec;
        }
        Vector3 averagePos = sum / strokesPositionList.Count;
        selectedStrokesCommonParent = Instantiate(new GameObject("common-parent"), averagePos, Quaternion.identity).transform;
        //selectedStrokesCommonParent.transform.position = averagePos;
        foreach(Transform t in selectedStrokes)
        {
            t.parent = selectedStrokesCommonParent;
        }
        foreach(GameObject g in listForSettingCenter)
        {
            Destroy(g);
        }
        potentialStrokeSelectionList.Clear();
        // common parent objects will be more and more every time we sketch. but i am not focused on optimizing.
        // for optimization, we can check if the common-parent is empty (has no children). if empty, we can destroy it
    }
    public void AddToPotentialStokeList(Transform _stroke)
    {
        ClearSelectedStrokes();
        if(potentialStrokeSelectionList.Contains(_stroke) == false)
        {
            //_stroke.GetComponent<MeshRenderer>().material.color = Color.gray;
            Color matColor = _stroke.GetComponent<MeshRenderer>().material.color;
            matColor.a = 0.25f; // become transparent if it is in the potential stoke list
            _stroke.GetComponent<MeshRenderer>().material.color = matColor;
            potentialStrokeSelectionList.Add(_stroke);
        }
    }

    public void ApproveStrokeSelections()
    {
        ClearSelectedStrokes();

        AssignSelectedStrokes();

        gazeReader.ChangeState(GazeReader.InteractionState.Editing);
        
        ManipulateSelectedStrokes();
    }


    public void EndStrokeManipulation()
    {
        ClearSelectedStrokes();
        ClearPotentialStrokes();
        
        StopManipulation();
    }

    private void ManipulateSelectedStrokes()
    {
        canManipulate = true;
    }

    private void StopManipulation()
    {
        canManipulate = false;
    }

    private void Update()
    {
        if(canManipulate == true && selectedStrokes.Count > 0)
        {
            foreach(Transform t in selectedStrokes)
            {
                //t.localPosition = t.localPosition + positionManipulator.GetMovementDirection * positionManipulator.GetMovementScale * Time.deltaTime;
            }

            foreach(Transform tScale in selectedStrokesForScaling)
            {
                //tScale.localScale = tScale.localScale + Vector3.one * scaleManipulator.GetValue * Time.deltaTime;
            }

            selectedStrokesCommonParent.localScale = selectedStrokesCommonParent.localScale + Vector3.one * scaleManipulator.GetValue * Time.deltaTime;
            selectedStrokesCommonParent.localPosition = selectedStrokesCommonParent.localPosition + positionManipulator.GetMovementDirection * positionManipulator.GetMovementScale * Time.deltaTime;
        }
    }
}
