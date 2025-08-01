using TMPro;
using UnityEngine;

public class GazeReader : MonoBehaviour
{
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private LayerMask eyesHitLayers;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private PositionManipulator positionManipulator;
    [SerializeField] private ScaleManipulator scaleManipulator;
    [SerializeField] private RotationManipulator rotationManipulator;

    [SerializeField] private StrokeManipulation strokeManipulation;

    private float colorPaletteCounter;
    [SerializeField] private ColorPaletteActivation colorPaletteActivation;
    private float colorPaletteCooldown = -1f;


    private float strokeApprovalCounter;
    [SerializeField] private DrawController drawController;
    private bool isSelecting;

    public enum InteractionState
    {
        Registrating,
        Drawing,
        Selecting,
        Editing
    }
    private InteractionState interactionState;
    public InteractionState GetInteractionState
    {
        get { return interactionState; }
    }
    [SerializeField] private TMP_Text stateText;

    [SerializeField] private SurfaceInteraction surfaceInteraction;
    private bool isLookingToSurface;
    private SurfaceInteraction.Point potentialPoint;
    private float counterForPoints;

    [SerializeField] private Registration registration;
    private float counterForRegistrationPoints;

    //[SerializeField] private DebugText debugText;
    private void Start()
    {
        ChangeState(InteractionState.Registrating);
    }
    private void Update()
    {
        Vector3 eyePositionAverage = (leftEye.position + rightEye.position) / 2f;
        Vector3 eyesForwardDirectionAverage = ((leftEye.forward + rightEye.forward) / 2f).normalized;

        RaycastHit hit;
        if(Physics.Raycast(eyePositionAverage, eyesForwardDirectionAverage, out hit, Mathf.Infinity, eyesHitLayers))
        {
            GameObject hitGameObject = hit.collider.gameObject;
            int objectLayerNumber = hitGameObject.layer;

            lineRenderer.SetPosition(0, eyePositionAverage);
            lineRenderer.SetPosition(1, hit.point);

            if(objectLayerNumber == 6)
            {
                hitGameObject.GetComponent<GazeFocus>().FocusWithGaze();
            }
            else if(objectLayerNumber == 8)
            {
                //Debug.Log($"COLOR BUTTON..");
                hitGameObject.GetComponent<ColorButton>().HoverToButton();
            }
            else if(objectLayerNumber == 9 && isSelecting)
            {
                // this is a stroke, add this in potential stroke selection list
                strokeManipulation.AddToPotentialStokeList(hitGameObject.transform);
            }
            else if(objectLayerNumber == 15 && interactionState == InteractionState.Drawing)
            {
                HitWall _hitWall = hitGameObject.GetComponent<HitWall>();
                potentialPoint.position = hit.point;
                potentialPoint.targetObject = hitGameObject;
                potentialPoint.direction = _hitWall.Direction;
                
            }
            
        }


        // selecting strokes
        if(strokeApprovalCounter > 0) strokeApprovalCounter -= Time.deltaTime;
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch) && strokeApprovalCounter <= 0f && interactionState == InteractionState.Selecting)
        {
            Debug.Log("button two pressed");
            strokeManipulation.ApproveStrokeSelections();
            strokeApprovalCounter = 1f;
        }

        // ending stroke manipulation
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.1f && (interactionState == InteractionState.Selecting || interactionState == InteractionState.Editing))
        {
            Debug.Log("secondary hand trigger pressed");
            strokeManipulation.EndStrokeManipulation();
            //drawController.IsManipulating = false;
            ChangeState(InteractionState.Drawing);
        }

        //
        if(interactionState == InteractionState.Drawing && OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Touch) && colorPaletteActivation.ColorPaletteActivated == false)
        {
            ChangeState(InteractionState.Selecting);
        }

        if(interactionState == InteractionState.Selecting)
        {
            if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Touch))
            {
                isSelecting = true;
            }
            else
            {
                isSelecting = false;
            }
        }

        // code for activating and deactivating the color palette. i will comment this to be able to use button B.
        // i will use B to put points on the surface.
        /*
        if(interactionState == InteractionState.Drawing && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            colorPaletteCounter += Time.deltaTime;
            Debug.Log($"count for palette... (counter: {colorPaletteCounter})");
            colorPaletteActivation.Counter = colorPaletteCounter;
            if(colorPaletteCounter >= 1f)
            {
                Debug.Log($"color palette counte reached to 1!!! Change states");
                colorPaletteCounter = -colorPaletteCooldown;
            }
        }
        else if(interactionState == InteractionState.Drawing && OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            Debug.Log($"reset state for color palette!!! user released the button! (last counter val: {colorPaletteCounter})");
            colorPaletteCounter = colorPaletteCooldown;
            colorPaletteActivation.Counter = colorPaletteCounter;
            colorPaletteActivation.ResetState();
        }
        */

        // for temporary it is left controller (right is charging)
        if(interactionState == InteractionState.Registrating && OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && counterForRegistrationPoints <= 0f)
        {
            /*
            // add registration point
            bool registrationCompleted = registration.AddPoint(drawController.GetBrush.GetController.position);
            counterForRegistrationPoints = 0.25f;
            if (registrationCompleted)
            {
                ChangeState(InteractionState.Drawing);
            }
            */

            
            registration.CalculateRoomPositionWithLeftController();
            ChangeState(InteractionState.Drawing);
            
            
        }
   
        if(counterForRegistrationPoints > 0f)
        {
            counterForRegistrationPoints -= Time.deltaTime;
        }


        // for temporary it is left controller (right is charging)
        if (interactionState == InteractionState.Drawing && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch) && counterForPoints <= 0f)
        {
            //Debug.Log($"ADD POINT");
            surfaceInteraction.AddPoint(potentialPoint);
            counterForPoints = 0.25f;
        }
        if(counterForPoints > 0f)
        {
            counterForPoints -= Time.deltaTime;
        }
        


    }

    public void ChangeState(InteractionState newState)
    {
        interactionState = newState;
        //debugText.EnterState(newState.ToString());
        switch (interactionState)
        {
            case InteractionState.Drawing:
                stateText.text = $"State: Drawing";
                positionManipulator.gameObject.SetActive(false);
                scaleManipulator.gameObject.SetActive(false);
                rotationManipulator.gameObject.SetActive(false);
                rotationManipulator.RotationDifference = Quaternion.identity;
                break;
            case InteractionState.Selecting:
                stateText.text = $"State: Selecting";
                positionManipulator.gameObject.SetActive(false);
                scaleManipulator .gameObject.SetActive(false);
                rotationManipulator.gameObject.SetActive(false);
                rotationManipulator.RotationDifference = Quaternion.identity;
                break;
            case InteractionState.Editing:
                stateText.text = $"State: Editing";
                Vector3 manipulatorDirection = Camera.main.transform.forward * 0.5f - Camera.main.transform.up * 0.25f + Camera.main.transform.right * 0.25f;
                positionManipulator.transform.position = Camera.main.transform.position + manipulatorDirection;
                scaleManipulator.transform.position = positionManipulator.transform.position - positionManipulator.transform.right * 0.3f;
                rotationManipulator.transform.position = scaleManipulator.transform.position - scaleManipulator.transform.right * 0.3f;
                positionManipulator.gameObject.SetActive(true);
                scaleManipulator.gameObject.SetActive(true);
                rotationManipulator.gameObject.SetActive(true);
                break;
        }
    }
}
