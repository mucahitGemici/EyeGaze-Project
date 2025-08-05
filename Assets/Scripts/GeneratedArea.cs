using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GeneratedArea : MonoBehaviour
{
    private Transform targetController;
    public Transform TargetController
    {
        set { targetController = value; }
    }
    public bool controlledByUser;
    Transform brush;
    public Transform Brush
    {
        set {  brush = value; }
    }

    private GeneratedArea controlledBy;
    public GeneratedArea ControlledBy
    {
        set { controlledBy = value; }
    }

    List<Transform> points = new List<Transform>();

    GameObject control;
    public GameObject GetControl
    {
        get { return control; }
    }

    private float counter;

    float minX = Mathf.Infinity;
    float maxX = -Mathf.Infinity;
    float minY = Mathf.Infinity;
    float maxY = -Mathf.Infinity;
    float minZ = Mathf.Infinity;
    float maxZ = -Mathf.Infinity;

    float relativeLocalX;
    float relativeLocalY;
    float relativeLocalZ;

    Vector3 desiredLocalPos;
    [SerializeField] Vector3 desiredPos;

    private Vector3 relativeLocalPosForArea;
    public Vector3 RelativeLocalPosForArea
    {
        get 
        {
            return relativeLocalPosForArea; 
        }
    }

    public Vector3 DesiredPos
    {
        set { 
            desiredPos = value;
        }
    }
    private void Start()
    {
        points = GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);

        


        foreach(Transform point in points)
        {
            Vector3 corner = point.localPosition;
            if (corner.x <= minX)
            {
                minX = corner.x;
            }
            if (corner.x >= maxX)
            {
                maxX = corner.x;
            }
            if (corner.y <= minY)
            {
                minY = corner.y;
            }
            if (corner.y >= maxY)
            {
                maxY = corner.y;
            }
            if (corner.z <= minZ)
            {
                minZ = corner.z;
            }
            if (corner.z >= maxZ)
            {
                maxZ = corner.z;
            }
        }

    }

    private void Update()
    {
        if (control == null) return;

        if (controlledByUser)
        {
            // follow controller position
            desiredPos = targetController.position;

            relativeLocalX = (control.transform.localPosition.x / (Mathf.Abs(minX) + Mathf.Abs(maxX)));
            relativeLocalY = (control.transform.localPosition.y / (Mathf.Abs(minY) + Mathf.Abs(maxY)));
            relativeLocalZ = (control.transform.localPosition.z / (Mathf.Abs(minZ) + Mathf.Abs(maxZ)));
            Vector3 relativePos = new Vector3(relativeLocalX, relativeLocalY, relativeLocalZ);

            Vector3 posForControl = ConvertWorldToLocal(desiredPos);

            //Debug.Log($"sent local pos: {relativePos}");
            relativeLocalPosForArea = relativePos;

            control.transform.localPosition = posForControl;
        }
        else
        {
            // follow wim (other generated area position)

            // sorun burada. digerinden aldigim konum bilgisini kullanamiyorum. muhtemelen relativePos yanlis hesaplaniyor.

            Vector3 desiredLocalPos = controlledBy.RelativeLocalPosForArea;
            desiredLocalPos.x *= (Mathf.Abs(minX) + Mathf.Abs(maxX));
            desiredLocalPos.y *= (Mathf.Abs(minY) + Mathf.Abs(maxY));
            desiredLocalPos.z *= (Mathf.Abs(minZ) + Mathf.Abs(maxZ));
            // desiredLocalPos is true. but can not use in position. why?
            //Debug.Log($"Received local pos: {desiredLocalPos}");
            control.transform.localPosition = desiredLocalPos;

            //control.transform.localPosition = desiredLocalPos;
            //control.transform.position = worldPos;
            brush.position = control.transform.position;
            
        }
    }

    
    

    private Vector3 ConvertWorldToLocal(Vector3 worldPoint)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint);
        
        if(local.x < minX)
        {
            local.x = minX;
        }

        if (local.x > maxX)
        {
            local.x = maxX;
        }

        if(local.y < minY)
        {
            local.y = minY;
        }

        if (local.y > maxY)
        {
            local.y = maxY;
        }

        if (local.z < minZ)
        {
            local.z = minZ;
        }

        if(local.z > maxZ)
        {
            local.z = maxZ;
        }

        //Debug.Log($"world: {desiresPos} = local: {local}");
        return local;
    }

    public void GenerateControl()
    {
        control = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        control.transform.localScale = Vector3.one * 0.05f;
        control.transform.parent = this.transform;
        control.transform.position = transform.position;

        MeshRenderer mr = control.GetComponent<MeshRenderer>();
        mr.material.color = Color.white;

        if (controlledByUser)
        {
            //mr.enabled = false;
        }
    }
}
