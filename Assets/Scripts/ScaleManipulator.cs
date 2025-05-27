using TMPro;
using UnityEngine;

public class ScaleManipulator : MonoBehaviour
{
    [SerializeField] private Transform holder;

    private bool isHoveringHolder;
    public bool IsHoveringHolder
    {
        set { isHoveringHolder = value; }
    }

    [SerializeField] private Transform brushTransform;

    private MeshRenderer mrHolder;
    private Color initialMrHolderColor;

    private bool isMoving;

    [SerializeField] private Transform transparentPart; 
    private float directLength;
    [SerializeField] private TMP_Text debugText;

    private float value;
    public float GetValue
    {
        get { return value; }
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
            Vector3 requestedPos = brushTransform.position;
            requestedPos.x = transform.position.x;
            requestedPos.z = transform.position.z;
            requestedPos.y = Mathf.Clamp(requestedPos.y, transform.position.y - directLength, transform.position.y + directLength);

            holder.transform.position = requestedPos;

            value = (holder.transform.position.y - transform.position.y)/directLength;

            
        }
        else
        {
            mrHolder.material.color = Color.red;
            holder.transform.position = transform.position;
            value = 0;
        }


        if (isMoving && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) == 0)
        {
            isMoving = false;
        }
    }
}
