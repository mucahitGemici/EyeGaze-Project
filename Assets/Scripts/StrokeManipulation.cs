using UnityEngine;
using System.Collections.Generic;
public class StrokeManipulation : MonoBehaviour
{
    private List<Transform> potentialStrokeSelectionList = new List<Transform>();

    private List<Transform> selectedStrokes = new List<Transform>();

    [SerializeField] private DrawController drawController;
    [SerializeField] private GazeReader gazeReader;
    [SerializeField] private Manipulator manipulator;

    private bool canManipulate = false;
    private void ClearSelectedStrokes()
    {
        foreach (Transform t in selectedStrokes)
        {
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque when selected strokes resetted
            t.GetComponent<MeshRenderer>().material.color = matColor;
        }
        selectedStrokes.Clear();
    }

    private void ClearPotentialStrokes()
    {
        foreach(Transform t in potentialStrokeSelectionList)
        {
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque when potential strokes resetted
            t.GetComponent<MeshRenderer>().material.color = matColor;
        }
        potentialStrokeSelectionList.Clear();
    }

    private void AssignSelectedStrokes()
    {
        foreach (Transform t in potentialStrokeSelectionList)
        {
            selectedStrokes.Add(t);
            Color matColor = t.GetComponent<MeshRenderer>().material.color;
            matColor.a = 1f; // become opaque after approving selection
            t.GetComponent<MeshRenderer>().material.color = matColor;
        }
        potentialStrokeSelectionList.Clear();
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
                t.position = t.position + manipulator.GetMovementDirection * manipulator.GetMovementScale * Time.deltaTime;
            }
        }
    }
}
