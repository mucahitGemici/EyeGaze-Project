using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class DebugText : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;

    private string stateInfo;
    private string controllerPos;
    private string posOffset;
    private string rotOffset;

    public void EnterState(string _stateInfo)
    {
        stateInfo = _stateInfo;
        PrintMessage();
    }

    public void EnterControllerPos(Vector3 pos)
    {
        controllerPos = $"x:{pos.x}, y:{pos.y}, z:{pos.z}";
        PrintMessage();
    }

    public void EnterPosOffset(Vector3 _posOffset)
    {
        posOffset = $"x:{_posOffset.x}, y:{_posOffset.y}, z:{_posOffset.z}";
        PrintMessage();
    }

    public void EnterRotOffset(Vector3 _rotOffset)
    {
        rotOffset = $"x:{_rotOffset.x}, y:{_rotOffset.y}, z:{_rotOffset.z}";
        PrintMessage();
    }

    private void PrintMessage()
    {
        debugText.text = $"state:{stateInfo}\ncontrollerPos:{controllerPos}\nposOffset:{posOffset}\nrotOffset:{rotOffset}";

    }

    
}
