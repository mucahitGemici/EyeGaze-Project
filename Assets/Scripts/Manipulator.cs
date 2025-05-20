using UnityEngine;

public class Manipulator : MonoBehaviour
{
    [SerializeField] private Transform holder;

    private bool isHoveringHolder;
    public bool IsHoveringHolder
    {
        set { isHoveringHolder = value; }
    }

    private Vector3 requestedMovement;
    [SerializeField] private Transform brushTransform;
    [SerializeField] private LineRenderer lineRenderer;

    private MeshRenderer mrHolder;
    private Color initialMrHolderColor;

    private bool isMoving;
    private void Start()
    {
        mrHolder = holder.GetComponent<MeshRenderer>();
        initialMrHolderColor = mrHolder.material.color;
    }
    private void Update()
    {
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0.1 && isHoveringHolder)
        {
            isMoving = true;

            
        }

        if (isMoving)
        {
            mrHolder.material.color = Color.green;

            //requestedMovement =  (transform.position - brushTransform.position).normalized * 0.5f;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, brushTransform.position);

            
        }
        else
        {
            mrHolder.material.color = Color.red;
        }

        if(isMoving && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) == 0)
        {
            isMoving = false;
        }

    }
}
