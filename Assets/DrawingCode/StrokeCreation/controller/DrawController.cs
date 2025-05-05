/* 
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
    
    private InputDevice rightHandDevice;
    private bool lastButtonState = false;

    public bool _drawing;
    private GameObject brush;

    public bool tempState=false;


    public void Start()
    {
        //get info for drawing
        _name = gameObject.name;
        _drawing = false;

        brush = transform.Find("Brush").gameObject;

        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.drawing;
    }

    public void onEnable()
    {
        //grab right hand controllers
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count == 1)
        {
            rightHandDevice = rightHandDevices[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", rightHandDevice.name, rightHandDevice.role.ToString()));
        }
        else if (rightHandDevices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        } else
        {
            Debug.Log("No Device found!");

        }
    }

    public void Update()
    {
        //send position for drawing
        EventManager.Instance.QueueEvent(new OnDrawingEvent(brush.transform));

        //check if trigger pressed
        bool triggerValue1 = false;
        bool tempState1 = rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out triggerValue1) // did get a value
                        && triggerValue1; // the value we got
        //Debug.Log(tempState.ToString() + " " + lastButtonState);

        //if state of the button changed since last frame
        if (tempState != lastButtonState) 
        {
            OnTriggerClick(tempState);
            lastButtonState = tempState;
        }

        if (tempState != lastButtonState)
        {
            OnTriggerClick(tempState);
            lastButtonState = tempState;
        }

        //this is the instruction to finish the try and store all the strokes as prefabs
        if (Input.GetKeyDown(KeyCode.N))
        {
            GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.finish;
            EventManager.Instance.TriggerEvent(new NewCanvasEvent());
        }
    }

    public void triggerPressed()
    {

        tempState = true;

    }

    public void triggerReleased()
    {

        tempState = false;

    }


    public void OnTriggerClick(bool value)
    {
        if (value)
        {
            Debug.Log("Draw!");

          //  if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
          //  {
                _drawing = true;
                EventManager.Instance.QueueEvent(new StartDrawingEvent(brush.transform));
           // }
        }

        else
        {
            Debug.Log(" STOP Draw!");

           // if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
           // {
                _drawing = false;
                EventManager.Instance.QueueEvent(new EndDrawingEvent(brush.transform));
            //}
        }

    }

}
