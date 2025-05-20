using UnityEngine;
using System.Collections.Generic;
public class StrokeManipulation : MonoBehaviour
{
    private List<Transform> potentialStrokeSelectionList = new List<Transform>();

    private List<Transform> selectedStrokes = new List<Transform>();

    [SerializeField] private DrawController drawController;
    [SerializeField] private GazeReader gazeReader;

   
    private void ClearSelectedStrokes()
    {
        foreach (Transform t in selectedStrokes)
        {
            t.GetComponent<MeshRenderer>().material.color = Color.black;
        }
        selectedStrokes.Clear();
    }

    private void ClearPotentialStrokes()
    {
        foreach(Transform t in potentialStrokeSelectionList)
        {
            t.GetComponent<MeshRenderer>().material.color = Color.black;
        }
        potentialStrokeSelectionList.Clear();
    }

    private void AssignSelectedStrokes()
    {
        foreach (Transform t in potentialStrokeSelectionList)
        {
            selectedStrokes.Add(t);
            t.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        potentialStrokeSelectionList.Clear();
    }
    public void AddToPotentialStokeList(Transform _stroke)
    {
        ClearSelectedStrokes();
        if(potentialStrokeSelectionList.Contains(_stroke) == false)
        {
            _stroke.GetComponent<MeshRenderer>().material.color = Color.gray;
            potentialStrokeSelectionList.Add(_stroke);
        }
    }

    public void ApproveStrokeSelections()
    {
        ClearSelectedStrokes();

        AssignSelectedStrokes();

        gazeReader.ChangeState(GazeReader.InteractionState.Editing);
    }


    public void EndStrokeManipulation()
    {
        ClearSelectedStrokes();
        ClearPotentialStrokes();
    }
}
