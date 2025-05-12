/* 
 * mayra barrera, 2017
 * script for stroke control + log
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ExperimentControl : MonoBehaviour {

    //this variable should show the current guide
    //private string currentDrawingObject;   
    //this dictionary has the list of guides, the key is the guide name
    //private Dictionary<string, GameObject> DrawnObjects = new Dictionary<string, GameObject>();

    private GameObject StrokeContainer;
    private GameObject UIContainer;

    private GameObject UserHead;
    private GameObject ControllerRight;
    private GameObject ControllerLeft;

    [SerializeField] private int participantID;

    private void Start()
    {
        //for test, remove in study
        #region test     
        GlobalVars.Instance.currentParticipant = $"{participantID}";
        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.seeModel;

        GlobalVars.Instance.thisObjectShape = GlobalVars.ObjectShape.circle;
        GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.noGuide;
        GlobalVars.Instance.thisDrawnDirection = GlobalVars.DrawnDirection.lateral;
        GlobalVars.Instance.thisDrawnSize = GlobalVars.DrawnSize.small;

        EventManager.Instance.TriggerEvent(new CreateXMLEvent(false, "", GlobalVars.Instance.currentParticipant));
        EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString()},
            GlobalVars.LogCategory.startTry, "went to view object"));
        #endregion

        //start the dictionary of guides (it needs a name and a gameobject)
        //DrawnObjects = new Dictionary<string, GameObject>();

        StrokeContainer = GameObject.Find("StrokeContainer");
        UIContainer = GameObject.Find("UIContainer");
        
        //this method is for loading all the guides in the dictionary
        //OnSceneLoad();
    }

    private void OnSceneLoad()
    {
        //loads all the guides into the dictionary before activating the current guide (I was lazy and had all the options as gameobjects on scene, and just turn on/off accordingly.
        //foreach (Transform child in UIContainer.transform)
        //{
        //    DrawnObjects.Add(child.name, child.gameObject);
        //    child.gameObject.SetActive(false);
        //}

        //show current drawing object
        //currentDrawingObject = GlobalVars.Instance.thisObjectShape.ToString();
        //DrawnObjects[currentDrawingObject].SetActive(true);

    }

    public void Phase(int strokeCount)
    {
        if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
        {
            EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                GlobalVars.LogCategory.startTry, "start try"));

            //start user position log
            UserHead = GameObject.Find("Camera");
            StartCoroutine("LogUserPose", 0.5F);
        }

        if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.finish)
        {
            EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                GlobalVars.LogCategory.endTry, "finish try_" + strokeCount));

            //end user position log
            StopCoroutine("LogUserPose");

            //ACTIVATE IN STUDY
            #region save objects //commented
            
            //save stroke meshFilter
            foreach (Transform child in StrokeContainer.transform)
            {
                MeshFilter mf = child.GetComponent<MeshFilter>() as MeshFilter;
                Mesh mesh = mf.sharedMesh;

                EventManager.Instance.TriggerEvent(new SaveStrokeEvent(GlobalVars.Instance.currentParticipant,
                   new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                   child.name, mesh));
            }
            
            #endregion

            //load start scene
            //SceneManager.LoadScene("startScene", LoadSceneMode.Single);
        }
    }

    public void StartStroke(Vector3 position)
    {

        EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
            GlobalVars.LogCategory.newStroke, "" + position));

        //if the guide needs to be hidden while drawing (for the no guide condition), this needs an if before
        //DrawnObjects[currentDrawingObject].SetActive(false);
    }

    public void EndStroke(Vector3 position)
    {
        //end user position log
        EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
            GlobalVars.LogCategory.endStroke, "" + position));

        //if the guide needs to be shown while not drawing (for the no guide condition), this needs an if before
        //DrawnObjects[currentDrawingObject].SetActive(true);
    }

    IEnumerator LogUserPose(float waitTime)
    {
        while (true)
        {
            //start log of controller position
            if (ControllerRight == null)
            {
                ControllerRight = GameObject.Find("Controller (right)");
            }

            if (ControllerLeft == null)
            {
                ControllerLeft = GameObject.Find("Controller (left)");
            }

            //USER HEAD POSE
            #region head
            string position = "" + UserHead.transform.position.x + "," + UserHead.transform.position.y + "," + UserHead.transform.position.z;

            Quaternion rotation = UserHead.transform.rotation;
            string rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            string viewNormal = UserHead.transform.forward.x + "," + UserHead.transform.forward.y + "," + UserHead.transform.forward.z;

            string pose = position + "/" + rotation + "/" + viewNormal;

            EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                GlobalVars.LogCategory.userPose, pose));
            #endregion

            //RIGHT CONTROLLER
            #region rightController
            position = "" + ControllerRight.transform.position.x + "," + ControllerRight.transform.position.y + "," + ControllerRight.transform.position.z;

            rotation = ControllerRight.transform.rotation;
            rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            viewNormal = ControllerRight.transform.forward.x + "," + ControllerRight.transform.forward.y + "," + ControllerRight.transform.forward.z;

            pose = position + "/" + rotation + "/" + viewNormal;

            EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                GlobalVars.LogCategory.rightControllerPose, pose));
            #endregion

            //LEFT CONTROLLER
            #region leftController
            position = "" + ControllerLeft.transform.position.x + "," + ControllerLeft.transform.position.y + "," + ControllerLeft.transform.position.z;

            rotation = ControllerLeft.transform.rotation;
            rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            viewNormal = ControllerLeft.transform.forward.x + "," + ControllerLeft.transform.forward.y + "," + ControllerLeft.transform.forward.z;

            pose = position + "/" + rotation + "/" + viewNormal;

            EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisDrawnDirection.ToString(), GlobalVars.Instance.thisDrawnSize.ToString() },
                GlobalVars.LogCategory.leftControllerPose, pose));
            #endregion

            yield return new WaitForSeconds(waitTime);
        }
    }
}
