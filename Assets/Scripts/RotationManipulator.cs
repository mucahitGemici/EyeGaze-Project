using UnityEngine;


public class RotationManipulator : MonoBehaviour
{
    [SerializeField] private Transform holder;

    private bool isHoveringHolder;
    public bool IsHoveringHolder
    {
        set { isHoveringHolder = value; }
    }

    [SerializeField] private Transform brushTransform;
    [SerializeField] private Transform controllerTransform;

    private MeshRenderer mrHolder;

    private Color initialMrHolderColor;

    private bool isMoving;

    [SerializeField] private Transform transparentPart;

    private Quaternion requestedRotation;
    public Quaternion RequestedRotation
    {
        get {  return requestedRotation; }
    }

    [HideInInspector] public Quaternion initialRot;

    Quaternion difference;
    public Quaternion RotationDifference
    {
        get { return difference; }
        set { difference = value; }
    }

    private Vector3 controllerDirection;
    private Vector3 borderPosition;
    private float directLength;

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


            // rotate holder
            //holder.rotation = brushTransform.rotation;
            holder.position = controllerTransform.position;
            difference = Quaternion.Lerp(difference, (Quaternion.Inverse(initialRot) * controllerTransform.rotation), 2.5f * Time.deltaTime);

            //holder.rotation = difference * holder.rotation;
            holder.rotation = difference;
            // reflect it to strokes (via a value getter)
            controllerDirection = (brushTransform.position - transform.position).normalized;
            borderPosition = transform.position + controllerDirection * directLength;
            float holderRange = (Mathf.Abs(Vector3.Magnitude(brushTransform.position - transparentPart.position))) / directLength;

            if (holderRange < 1f)
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
            //holder.transform.rotation = transform.rotation;
            holder.transform.position = transform.position;
        }

        if (isMoving && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) == 0)
        {
            isMoving = false;
        }
    }
}
