/* Mayra Barrera
*  September, 2015 
*  class to save log in an xml
*/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Xml;

public class XMLWriter : MonoBehaviour {

	private string directory;
	private string logFilePath;

	void Awake(){
        EventManager.Instance.AddListener<CreateXMLEvent>(CreateXML);
        EventManager.Instance.AddListener<CreateEntryEvent>(CreateEntry);
        EventManager.Instance.AddListener<EndLogEvent>(EndLog);
        EventManager.Instance.AddListener<TakeScreenshotEvent>(TakeScreenshot);
        EventManager.Instance.AddListener<SaveStrokeEvent>(SaveStroke);
	}

    private void CreateXML(CreateXMLEvent e)
    {
        //create folders to save logs
        directory = "Test" + "/log";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //create xml file
        XmlWriterSettings wrapperSettings = new XmlWriterSettings();
        wrapperSettings.Indent = true;

        //write head file
        string wrapperName;
        if (!e.specificName)
        {
            string baseName = DateTime.Now.ToString("h-mm-ss");
            wrapperName = directory + "/wrapper_" + e.participantName.ToString() + "_" + baseName + ".xml";
            logFilePath = directory + "/log_" + e.participantName.ToString() + "_" + baseName + ".xml";
        }

        else
        {
            wrapperName = directory + "/wrapper_" + e.name + ".xml";
            logFilePath = directory + "/log_" + e.name + ".xml";
        }

        using (XmlWriter writer = XmlWriter.Create(wrapperName, wrapperSettings))
        {

            writer.WriteStartDocument();
            writer.WriteStartElement("logData");

            //meta
            writer.WriteStartElement("meta");

            writer.WriteStartElement("startTime");
            writer.WriteValue(DateTime.Now);
            writer.WriteEndElement();

            writer.WriteStartElement("participant");
            writer.WriteValue(e.participantName);
            writer.WriteEndElement();

            writer.Close();
        }

        GlobalVars.Instance.logFile = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        GlobalVars.Instance.fragmentSetting = new XmlWriterSettings();
        GlobalVars.Instance.fragmentSetting.ConformanceLevel = ConformanceLevel.Fragment;
    }

    //create record based on info received
    private void CreateEntry(CreateEntryEvent e)
    {

        using (XmlWriter writer1 = XmlWriter.Create(GlobalVars.Instance.logFile, GlobalVars.Instance.fragmentSetting))
        {
            //write info in a single line
            string newLine = DateTime.Now.ToString("h:mm:ss.fff") +
                        ";" + e.participantName +
                        ";" + e.experimentVariables[0] + //shape
                        ";" + e.experimentVariables[1] + //visual guide  
                        ";" + e.experimentVariables[2] + //drawn direction
                        ";" + e.experimentVariables[3] + //drawn size
                        ";" + e.category +
                        ";" + e.value;

            writer1.WriteStartElement("newEntry");
            writer1.WriteValue(newLine);
            writer1.WriteEndElement();

            writer1.Flush();
        }
        GlobalVars.Instance.logFile.Flush();
    }

    //close file
    private void EndLog(EndLogEvent E){
        GlobalVars.Instance.logFile.Close();
	}

    //Take and Save Screenshot
    private void TakeScreenshot(TakeScreenshotEvent e)
    {

        string imageName = "Test/log/screenshot_" + e.participantName.ToString() + "_" + e.experimentVariables[0] + "_" + e.experimentVariables[1] + 
            "_" + e.experimentVariables[2] + 
            "_" + e.CameraName + ".png";

        RenderTexture rt = new RenderTexture(e.WidthResolution, e.HeightResolution, 24);

        Camera myCamera = GameObject.Find(e.CameraName).GetComponent<Camera>();
        myCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(e.WidthResolution, e.HeightResolution, TextureFormat.RGB24, false);
        myCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, e.WidthResolution, e.HeightResolution), 0, 0);
        myCamera.targetTexture = null;

        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();

       File.WriteAllBytes(imageName, bytes);
       
        Debug.Log(string.Format("Took screenshot to: {0}", imageName));
    }

    private void SaveStroke(SaveStrokeEvent e)
    {
        string strokeName = "stroke_"  + e.participantName.ToString() + "_" + e.experimentVariables[0] + "_" + e.experimentVariables[1]
            + "_" + e.experimentVariables[2] + "_" + e.experimentVariables[3]
            + "_" + e.meshName;

        string pathInsideAssets = "Assets/LOGs/study/" + e.participantName.ToString();

        if (!Directory.Exists(pathInsideAssets))
        {
            Directory.CreateDirectory(pathInsideAssets);
        }

        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", pathInsideAssets, strokeName, "asset");
        path = FileUtil.GetProjectRelativePath(path);

        MeshUtility.Optimize(e.mesh);


        try
        {
            AssetDatabase.CreateAsset(e.mesh, path);
            AssetDatabase.SaveAssets();
        }
        catch(InvalidCastException eX)
        {
            Debug.Log(eX.ToString());
        }

        strokeName = "gameObject_" + e.participantName.ToString() + "_" + e.experimentVariables[0] + "_" + e.experimentVariables[1]
            + "_" + e.experimentVariables[2] + "_" + e.experimentVariables[3]
            + "_" + e.meshName;

        string nameLocalPath = pathInsideAssets + "/" + strokeName + ".prefab";

        UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(nameLocalPath);
        PrefabUtility.ReplacePrefab(GameObject.Find(e.meshName), prefab, ReplacePrefabOptions.ConnectToPrefab);

        //Debug.Log(string.Format("Saved Mesh: {0}", strokeName));
    }
}
