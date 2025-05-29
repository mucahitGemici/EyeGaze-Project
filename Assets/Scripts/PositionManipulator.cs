using UnityEngine;

public class PositionManipulator : MonoBehaviour
{
    [SerializeField] private Transform holder;

    private bool isHoveringHolder;
    public bool IsHoveringHolder
    {
        set { isHoveringHolder = value; }
    }

    [SerializeField] private Transform brushTransform;
    [SerializeField] private LineRenderer lineRenderer;
    //[SerializeField] private TMPro.TMP_Text testingText;

    private MeshRenderer mrHolder;
    private Color initialMrHolderColor;

    private bool isMoving;

    [SerializeField] private Transform transparentPart; // sphere
    private float directLength; // half of sphere

    private Vector3 controllerDirection;
    private Vector3 borderPosition;
    private float holderRange;

    public Vector3 GetMovementDirection
    {
        get { return controllerDirection; }
    }
    public float GetMovementScale
    {
        get
        {
            if(holderRange < 1f)
            {
                return holderRange;
            }
            else
            {
                return 1f;
            }
        }
    }
    private void Start()
    {
        mrHolder = holder.GetComponent<MeshRenderer>();
        initialMrHolderColor = mrHolder.material.color;

        directLength = transparentPart.localScale.x / 2f;
    }
    private void Update()
    {

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0.1 && isHoveringHolder)
        {
            isMoving = true;

            
        }

        if (isMoving)
        {
            mrHolder.material.color = Color.green;

            controllerDirection = (brushTransform.position - transform.position).normalized;
            borderPosition = transform.position + controllerDirection * directLength;          
            holderRange = (Mathf.Abs(Vector3.Magnitude(brushTransform.position - transparentPart.position))) / directLength;
            //testingText.text = $"Direction: {controllerDirection}\nLineLength: {holderRange}";

            if(holderRange < 1f)
            {
                holder.position = brushTransform.position;
            }
            else
            {
                holder.position = borderPosition;
            }

        }
        else
        {
            mrHolder.material.color = Color.red;
            controllerDirection = Vector3.zero;
            borderPosition = transform.position;
            holderRange = 0;
            holder.position = transform.position;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, holder.position);

        if (isMoving && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) == 0)
        {
            isMoving = false;
        }

    }
}
