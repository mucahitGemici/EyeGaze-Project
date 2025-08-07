using TMPro;
using UnityEngine;

public class GazeReader : MonoBehaviour
{
    [SerializeField] private Brush brush;
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
        // do not use registration (no passthrough, no registration)
        Registrating,
        Drawing,
        Selecting,
        Editing,
        DrawingOnCanvas,
        DrawingOnTargetArea
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
    public SurfaceInteraction.Point PotentialPoint
    {
        get { return potentialPoint; }
    }
    private float counterForPoints;

    // no registration, but keep it here in case we need it
    [SerializeField] private Registration registration;
    private float counterForRegistrationPoints;

    //[SerializeField] private DebugText debugText;

    [SerializeField] private Transform testSphere;
    private void Start()
    {
        ChangeState(InteractionState.Drawing);
    }
    private void Update()
    {
        /*
        // how to restrict brush pos
        Vector3 targetPosBrush = brush.transform.position;
        if(targetPosBrush.x < surfaceInteraction.GetSelectedArea.maxX &&
            targetPosBrush.x > surfaceInteraction.GetSelectedArea.minX &&
            targetPosBrush.y < surfaceInteraction.GetSelectedArea.maxY &&
            targetPosBrush.y > surfaceInteraction.GetSelectedArea.minY &&
            targetPosBrush.z < surfaceInteraction.GetSelectedArea.maxZ &&
            targetPosBrush.z > surfaceInteraction.GetSelectedArea.minZ)
        {
            testSphere.transform.position = targetPosBrush;
        }
        */

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
                //potentialPoint.direction = _hitWall.Direction; no longer used
                potentialPoint.normal = hit.normal;
                
            }
            else
            {
                potentialPoint = new SurfaceInteraction.Point();
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

        // ending stroke manipulation (also check for drawing state to reset the position offset of the brush)
        // this can be resetting button. it can reset everything (indirect, direct sketching, etc)
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.1f && (interactionState == InteractionState.Selecting || interactionState == InteractionState.Editing || interactionState == InteractionState.Drawing))
        {
            Debug.Log("secondary hand trigger pressed");
            strokeManipulation.EndStrokeManipulation();
            //drawController.IsManipulating = false;
            
            brush.PositionOffset = Vector3.zero;
            surfaceInteraction.ClearIndirectSketching();
            drawController.ClearLineRendererDrawings();
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

        // no registation for now
        /*
        // // registration
        if(interactionState == InteractionState.Registrating && OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) && counterForRegistrationPoints <= 0f)
        {
            
            // registration with putting points irl
            // add registration point
            //bool registrationCompleted = registration.AddPoint(drawController.GetBrush.GetController.position);
            //counterForRegistrationPoints = 0.25f;
            //if (registrationCompleted)
            //{
            //    ChangeState(InteractionState.Drawing);
            //}
            

            // registration with controller
            //registration.CalculateRoomPositionWithLeftController();
            //ChangeState(InteractionState.Drawing);
            
            
        }
   
        if(counterForRegistrationPoints > 0f)
        {
            counterForRegistrationPoints -= Time.deltaTime;
        }
        */


        // for temporary it is left controller (right is charging)
        if (interactionState == InteractionState.Drawing && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch) && counterForPoints <= 0f && potentialPoint.normal != Vector3.zero)
        {
            //Debug.Log($"ADD POINT");
            Debug.Log($"adding point... pos: {potentialPoint.position} and normal: {potentialPoint.normal}");
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
                brush.isDrawingOnArea = false;
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
            case InteractionState.DrawingOnCanvas:
                // get surface position and normal
                Vector3 canvasPos = surfaceInteraction.GetSelectedCanvas.transform.position;
                //GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //test.transform.position = canvasPos;
                //test.transform.localScale = Vector3.one * 0.25f;

                // put an offset to the brush
                brush.isDrawingOnArea = false;
                brush.PositionOffset = (canvasPos - brush.transform.position) + surfaceInteraction.GetSelectedCanvas.normal * 0.25f;
                
                ChangeState(InteractionState.Drawing);
                break;
            case InteractionState.DrawingOnTargetArea:
                Vector3 canvasPosition = surfaceInteraction.GetSelectedCanvas.transform.position;
                //brush.PositionOffset = (canvasPosition - brush.transform.position) + surfaceInteraction.GetSelectedCanvas.normal * 0.25f;
                //brush.SetMovementArea = surfaceInteraction.GetSelectedArea;
                brush.isDrawingOnArea = true;
                ChangeState(InteractionState.Drawing);
                break;
        }
    }
}
