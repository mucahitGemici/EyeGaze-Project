using TMPro;
using UnityEngine;

public class GazeReader : MonoBehaviour
{
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private LayerMask eyesHitLayers;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Manipulator manipulator;

    [SerializeField] private StrokeManipulation strokeManipulation;

    private float colorPaletteCounter;
    [SerializeField] private ColorPaletteActivation colorPaletteActivation;
    private float colorPaletteCooldown = -1f;


    private float strokeApprovalCounter;
    [SerializeField] private DrawController drawController;
    private bool isSelecting;

    public enum InteractionState
    {
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
    private void Start()
    {
        ChangeState(InteractionState.Drawing);
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
        


    }

    public void ChangeState(InteractionState newState)
    {
        interactionState = newState;
        switch (interactionState)
        {
            case InteractionState.Drawing:
                stateText.text = $"State: Drawing";
                manipulator.gameObject.SetActive(false);
                break;
            case InteractionState.Selecting:
                stateText.text = $"State: Selecting";
                manipulator.gameObject.SetActive(false);
                break;
            case InteractionState.Editing:
                stateText.text = $"State: Editing";
                Vector3 manipulatorDirection = Camera.main.transform.forward * 0.5f - Camera.main.transform.up * 0.25f + Camera.main.transform.right * 0.25f;
                manipulator.transform.position = Camera.main.transform.position + manipulatorDirection;
                manipulator.gameObject.SetActive(true);
                break;
        }
    }
}
