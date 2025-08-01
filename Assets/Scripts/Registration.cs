using NUnit.Framework.Constraints;
using UnityEngine;

public class Registration : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform roomParent;
    private Transform point1 = null;
    private Transform point2 = null;
    private Transform point3 = null;

    private Vector3 desiredRoomPos;
    private Vector3 desiredRoomRot;
    private bool isDesiredValuesFounded;

    public float rightOffset;
    public float forwardOffset;

    [SerializeField] private LineRenderer lineRenderer;
    public Vector3 posOffset;
    public Vector3 dirOffset;

    [SerializeField] private AudioSource beepSound;

    //[SerializeField] private DebugText debugText;
    [SerializeField] private MeshRenderer[] meshRenderers;

    [SerializeField] private test _test;
    public bool AddPoint(Vector3 pos)
    {
        // returns TRUE after putting THIRD REGISTRATION POINT
        if(point1 == null)
        {
            Debug.Log("ADDING POINT 1");
            GameObject _point = Instantiate(pointPrefab, pos, Quaternion.identity);
            point1 = _point.transform;
            return false;
        }
        else if(point2 == null)
        {
            Debug.Log("ADDING POINT 2");
            GameObject _point = Instantiate(pointPrefab, pos, Quaternion.identity);
            point2 = _point.transform;
            return false;
        }
        else if(point3 == null)
        {
            Debug.Log("ADDING POINT 3");
            GameObject _point = Instantiate(pointPrefab, pos, Quaternion.identity);
            point3 = _point.transform;
            CalculateRoomPosition();
            return true;
        }
        return false;
    }
    private void CalculateRoomPosition()
    {
        Debug.Log("SETTING THE ROOM LOCATION");
        // these points will be placed by the user

        Vector3 averagePos = (point1.position + point2.position + point3.position) / 3f;
        //Debug.LogWarning($"AVERAGE POS: {averagePos} - Room Parent Pos: {roomParent.position} - Room Parent Direction: {roomParent.rotation.eulerAngles} - Room Parent Forward: {roomParent.forward}");
        roomParent.position = averagePos;

        // calculate 0,0,1 direction with points
        Vector3 dir = (point2.position - point3.position).normalized;
        //Debug.LogWarning($"calculated dir: {dir}");


        roomParent.forward = dir;
        //roomParent.position += roomParent.right * 0.204f + roomParent.forward * (-0.38f);

        desiredRoomPos = roomParent.position;
        desiredRoomRot = roomParent.rotation.eulerAngles;
        isDesiredValuesFounded = true;
    }
    public void CalculateRoomPositionWithLeftController()
    {
        Vector3 controllerLoc = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion controllerRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        lineRenderer.SetPosition(0, controllerLoc);
        lineRenderer.SetPosition(1, controllerLoc + controllerRot.eulerAngles.normalized * 1f);

        desiredRoomPos = controllerLoc;
        desiredRoomRot = controllerRot.eulerAngles;

        //debugText.EnterControllerPos(controllerLoc);
        //debugText.EnterPosOffset(posOffset);
        //debugText.EnterRotOffset(dirOffset);
        beepSound.Play();

        _test.Spawn();
        Invoke(nameof(DisableWallVisuals), 3f);

    }

    private void Update()
    {
        // registration is disabled. i am just spawning user (and starting experiment from the same physical location)
        // in case of future use of registration, i do not remove registration codes.
        // if this way is finalized, remove registration codes to avoid misunderstanding for future people.
        //roomParent.position = desiredRoomPos + posOffset;
        //roomParent.rotation = Quaternion.Euler(desiredRoomRot + dirOffset);
    }

    private void DisableWallVisuals()
    {

        foreach(MeshRenderer mr in meshRenderers)
        {
            //mr.enabled = false;
        }
    }
}
