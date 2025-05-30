﻿/* 
 * mayra barrera, 2021
 * script for drawing control
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DrawController : MonoBehaviour
{
    public string _name;
    
    //private InputDevice rightHandDevice;
    private bool lastButtonState = false;

    public bool _drawing;
    public GameObject brush;
    [HideInInspector] public bool firstStrokeStarted = false;
    private Brush brushScript;

    //public bool tempState=false;
    private bool triggerButtonState;
    public bool GetTriggerButtonState
    {
        get { return triggerButtonState; }
    }


    [SerializeField] private GazeReader _gazeReader;


    public void Start()
    {
        //get info for drawing
        _name = gameObject.name;
        _drawing = false;

        //brush = transform.Find("Brush").gameObject;
        brushScript = brush.GetComponent<Brush>();

        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.drawing;
    }

   

    public void Update()
    {
        //send position for drawing
        EventManager.Instance.QueueEvent(new OnDrawingEvent(brush.transform));

        //check if trigger pressed
        triggerButtonState = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0.1;
        if (_gazeReader.GetInteractionState != GazeReader.InteractionState.Drawing)
        {
            triggerButtonState = false;
        }
        //Debug.Log(tempState.ToString() + " " + lastButtonState);

        //if state of the button changed since last frame
        if (triggerButtonState != lastButtonState) 
        {
            OnTriggerClick(triggerButtonState);
            lastButtonState = triggerButtonState;
        }

        //this is the instruction to finish the try and store all the strokes as prefabs
        if (Input.GetKeyDown(KeyCode.N))
        {
            GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.finish;
            EventManager.Instance.TriggerEvent(new NewCanvasEvent());
        }
    }



    public void OnTriggerClick(bool value)
    {
        if (value)
        {
            Debug.Log("Draw!");

          //  if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
          //  {
                _drawing = true;
                brushScript.SetBrushState = Brush.BrushState.Using;
                if(firstStrokeStarted == false) firstStrokeStarted = true;
                EventManager.Instance.QueueEvent(new StartDrawingEvent(brush.transform));
           // }
        }

        else
        {
            Debug.Log(" STOP Draw!");
           // if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
           // {
                _drawing = false;
                brushScript.SetBrushState = Brush.BrushState.Ready;
                EventManager.Instance.QueueEvent(new EndDrawingEvent(brush.transform));
            //}
        }

    }

}
