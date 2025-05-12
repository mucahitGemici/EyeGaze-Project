using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.AI;
public class ObserverManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pidText;
    private string dataPath = $"Assets/Resources/LOGs/study";
    private List<GameObject> spawnedStrokes = new List<GameObject>();
    private List<GameObject> participantParents = new List<GameObject>();
    private int participantIndex;
    [Serializable]
    struct Drawing
    {
        public int _pid;
        public List<GameObject> strokes;

    }

    [SerializeField] private List<Drawing> _drawings = new List<Drawing>();

    private void Start()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(dataPath);
        try
        {
            if (directoryInfo.Exists)
            {
                //Debug.Log("it exist");
                //Debug.Log(directoryInfo.FullName);
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach(DirectoryInfo dirInfo in directoryInfos)
                {
                    //Debug.Log(dirInfo.FullName);
                    int participantID = int.Parse(dirInfo.Name);
                    //Debug.Log($"participant ID: {participantID}");

                    Drawing drawing = new Drawing();
                    drawing._pid = participantID;
                    drawing.strokes = new List<GameObject>();

                    string prefabPath = $"{dataPath}/{participantID}/Prefabs";
                    //Debug.Log($"look for the path (for prefabs) => {prefabPath}");
                    DirectoryInfo prefabDirInfo = new DirectoryInfo(prefabPath);

                    FileInfo[] fileInfos = prefabDirInfo.GetFiles();
                    List<FileInfo> prefabFileInfoList = new List<FileInfo>();
                    foreach(FileInfo info  in fileInfos)
                    {
                        //Debug.Log($"extension: {info.Extension}");
                        if(info.Extension == ".prefab")
                        {
                            prefabFileInfoList.Add(info);
                        }
                    }
                    List<string> prefabNames = new List<string>();
                    foreach(var pName in prefabFileInfoList)
                    {
                        //prefabNames.Add(pName.)
                    }
                    foreach(FileInfo fileInfo in prefabFileInfoList)
                    {
                        string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        GameObject stroke = Resources.Load($"LOGs/study/{participantID}/Prefabs/{nameWithoutExtension}") as GameObject;
                        //Debug.Log($"Processed: {stroke.name}");
                        drawing.strokes.Add(stroke);
                    }
                    _drawings.Add(drawing);

                }
            }

            
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }

        SpawnStrokes();
    }

    private void SpawnStrokes()
    {
        pidText.text = $"Participant ID: {_drawings[0]._pid}";
        participantIndex = 0;

        foreach(Drawing drawing in _drawings)
        {
            GameObject participantParent = new GameObject($"{drawing._pid}");
            if (participantParents.Count != 0) participantParent.SetActive(false);
            participantParents.Add(participantParent);
            foreach (GameObject stroke in drawing.strokes)
            {
                GameObject strokeInstance = Instantiate(stroke);
                strokeInstance.transform.SetParent(participantParent.transform);
            }
        }
        
       
    }


    public void NextP()
    {
        participantParents[participantIndex].SetActive(false);
        
        if(participantIndex + 1 > _drawings.Count - 1)
        {
            participantIndex = 0;
        }
        else
        {
            participantIndex++;
        }
        pidText.text = $"Participant ID: {_drawings[participantIndex]._pid}";
        participantParents[participantIndex].SetActive(true);
    }

    public void PrevP()
    {
        participantParents[participantIndex].SetActive(false);
        Debug.Log($"participantIndex - 1: {participantIndex - 1}");

        if(participantIndex - 1 < 0)
        {
            participantIndex = _drawings.Count - 1;
        }
        else
        {
            participantIndex--;
        }

        pidText.text = $"Participant ID: {_drawings[participantIndex]._pid}";
        participantParents[participantIndex].SetActive(true);
    }

    
}
