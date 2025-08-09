using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SurfaceInteraction : MonoBehaviour
{
    [SerializeField] private DrawController drawController;
    [SerializeField] private Brush brush;
    [SerializeField] private ExperimentManager experimentManager;
    [SerializeField] private GazeReader gazeReader;
    public enum PDirection
    {
        Right,
        Left,
        Front,
        Back
    }
    [Serializable]
    public struct Point
    {
        public Vector3 position;
        public GameObject targetObject;
        public PDirection direction;
        public Vector3 normal;
    }
    private Point[] points = new Point[2];

    [SerializeField] private Canvas exampleImage;
    [SerializeField] private GameObject pointPrefab;

    private GameObject pointHolder1;
    private GameObject pointHolder2;
    private Point potentialPoint1;
    private Point potentialPoint2;
    

    public struct SelectedCanvas
    {
        public Transform transform;
        public Vector3 normal;
        public float width;
        public float height;
    }
    private SelectedCanvas selectedCanvas;
    public SelectedCanvas GetSelectedCanvas
    {
        get { return selectedCanvas; }
    }

   public struct SelectedArea
    {
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;
    }
    private SelectedArea selectedArea;
    public SelectedArea GetSelectedArea
    {
        get { return selectedArea; }
    }

    private GameObject targetAreaObj;
    private GameObject wimObj;
    private GeneratedArea targetArea;
    private GeneratedArea wim;
    private Canvas localCanvas;

    [SerializeField] private LineRenderer possibleAreaLR1;
    
    public void AddPoint(Point pt)
    {
        // returns if the operation is successful or not
        // if we try to add more points than 2, it will return false

        /*
        // old (selecting points on the same object. but this is old. we do not select points on walls anymore.
        if (points[0].targetObject == null)
        {
            points[0] = pt;
            pointHolder = Instantiate(pointPrefab, pt.position, Quaternion.identity);
        }
        else if(points[1].targetObject == null && points[0].targetObject == pt.targetObject)
        {
            points[1] = pt;
            Instantiate(pointPrefab, pt.position, Quaternion.identity);
            // then put the box
            CreateBox2D();
        }
        else
        {
            Debug.LogWarning($"REMOVING POINTS: Point 1 = {points[0].targetObject} - Point 2 = {pt.targetObject}");
            Destroy(pointHolder);
            pointHolder = null;
            points = new Point[2];
        }
        */

        // new (selecting if the points have same normal)

        if (points[0].targetObject == null)
        {
            points[0] = pt;
            pointHolder1 = Instantiate(pointPrefab, pt.position, Quaternion.identity);
            potentialPoint1 = pt;
        }
        else if(points[1].targetObject == null && points[0].normal == pt.normal)
        {
            points[1] = pt;
            pointHolder2 = Instantiate(pointPrefab, pt.position, Quaternion.identity);
            potentialPoint2 = pt;
            // then put the box
            //CreateBox2D();
            CreateBox2D_New();

        }
        else
        {
            Destroy(pointHolder1);
            pointHolder1 = null;
            points = new Point[2];
        }
        
        return;

    }

   
    private void CreateBox2D_New()
    {
        Destroy(pointHolder1);
        Destroy(pointHolder2);
        pointHolder1 = null;
        pointHolder2 = null;

        localCanvas = Instantiate(exampleImage);
        localCanvas.renderMode = RenderMode.WorldSpace;

        RectTransform localRectTransform = localCanvas.GetComponent<RectTransform>();
        localRectTransform.position = Vector3.zero;
        localRectTransform.sizeDelta = new Vector2(1, 1);
        localRectTransform.anchoredPosition3D = points[0].position;

        Image localImage = localCanvas.GetComponentInChildren<Image>();

        float width = 0;
        float height = 0;

        Debug.Log($"NORMAL: {points[0].normal}");

        if (points[0].normal.x != 0 && points[0].normal.x < 0)
        {
            // RIGHT
            width = Mathf.Abs(points[0].position.z - points[1].position.z);
            height = Mathf.Abs(points[0].position.y - points[1].position.y);
            //Debug.Log($"x: {width}, y: {height}");
            localRectTransform.sizeDelta = new Vector2(width, height);
            localRectTransform.rotation = Quaternion.Euler(0, 90, 0);

            if (points[0].position.y > points[1].position.y && points[0].position.z > points[1].position.z)
            {
                // first "right top" was selected, then "left bottom". RIGHT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(0.01f, height / 2f, width / 2f);
            }
            else if (points[0].position.y > points[1].position.y && points[0].position.z < points[1].position.z)
            {
                // first "left top" was selected, then "right bottom". RIGHT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(0.01f, height / 2f, -width / 2f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.z > points[1].position.z)
            {
                // first "right bottom" was selected, then "left top". RIGHT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(0.01f, -height / 2f, width / 2f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.z < points[1].position.z)
            {
                // first "left bottom" was selected, then "right top". RIGHT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(0.01f, -height / 2f, -width / 2f);
            }
        }
        else if (points[0].normal.x != 0 && points[0].normal.x > 0)
        {
            // LEFT
            width = Mathf.Abs(points[0].position.z - points[1].position.z);
            height = Mathf.Abs(points[0].position.y - points[1].position.y);
            //Debug.Log($"x: {width}, y: {height}");
            localRectTransform.sizeDelta = new Vector2(width, height);
            localRectTransform.rotation = Quaternion.Euler(0, -90, 0);

            if (points[0].position.y > points[1].position.y && points[0].position.z > points[1].position.z)
            {
                // first "right top" was selected, then "left bottom". LEFT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, height / 2f, width / 2f);
            }
            else if (points[0].position.y > points[1].position.y && points[0].position.z < points[1].position.z)
            {
                // first "left top" was selected, then "right bottom". LEFT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, height / 2f, -width / 2f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.z > points[1].position.z)
            {
                // first "right bottom" was selected, then "left top". LEFT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, -height / 2f, width / 2f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.z < points[1].position.z)
            {
                // first "left bottom" was selected, then "right top". LEFT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, -height / 2f, -width / 2f);
            }

        }
        else if (points[0].normal.y != 0 && points[0].normal.y < 0)
        {
            // top (ceiling)
            width = Mathf.Abs(points[0].position.x - points[1].position.x);
            height = Mathf.Abs(points[0].position.z - points[1].position.z);
            localRectTransform.sizeDelta = new Vector2(width, height);
            localRectTransform.rotation = Quaternion.Euler(-90, 0, 0);

            if (points[0].position.z > points[1].position.z && points[0].position.x > points[1].position.x)
            {
                // first "right top" was selected, then "left bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, 0.02f, height / 2f);
            }
            else if (points[0].position.z > points[1].position.z && points[0].position.x < points[1].position.x)
            {
                // first "left top" was selected, then "right bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, 0.02f, height / 2f);
            }
            else if (points[0].position.z < points[1].position.z && points[0].position.x > points[1].position.x)
            {
                // first "right bottom" was selected, then "left top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, 0.02f, -height / 2f);
            }
            else if (points[0].position.z < points[1].position.z && points[0].position.x < points[1].position.x)
            {
                // first "left bottom" was selected, then "right top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, 0.02f, -height / 2f);
            }
        }
        else if (points[0].normal.y != 0 && points[0].normal.y > 0)
        {
            // bottom (surface)
            width = Mathf.Abs(points[0].position.x - points[1].position.x);
            height = Mathf.Abs(points[0].position.z - points[1].position.z);
            localRectTransform.sizeDelta = new Vector2(width, height);
            localRectTransform.rotation = Quaternion.Euler(90, 0, 0);

            if (points[0].position.z > points[1].position.z && points[0].position.x > points[1].position.x)
            {
                // first "right top" was selected, then "left bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -0.02f, height / 2f);
            }
            else if (points[0].position.z > points[1].position.z && points[0].position.x < points[1].position.x)
            {
                // first "left top" was selected, then "right bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -0.02f, height / 2f);
            }
            else if (points[0].position.z < points[1].position.z && points[0].position.x > points[1].position.x)
            {
                // first "right bottom" was selected, then "left top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -0.02f, -height / 2f);
            }
            else if (points[0].position.z < points[1].position.z && points[0].position.x < points[1].position.x)
            {
                // first "left bottom" was selected, then "right top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -0.02f, -height / 2f);
            }
        }
        else if (points[0].normal.z != 0 && points[0].normal.z < 0)
        {
            // front
            width = Mathf.Abs(points[0].position.x - points[1].position.x);
            height = Mathf.Abs(points[0].position.y - points[1].position.y);
            Debug.Log($"FOR FRONT => x: {width}, y: {height}");
            localRectTransform.sizeDelta = new Vector2(width, height);

            if (points[0].position.y > points[1].position.y && points[0].position.x > points[1].position.x)
            {
                // first "right top" was selected, then "left bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, height / 2f, 0.01f);
            }
            else if (points[0].position.y > points[1].position.y && points[0].position.x < points[1].position.x)
            {
                // first "left top" was selected, then "right bottom". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, height / 2f, 0.01f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.x > points[1].position.x)
            {
                // first "right bottom" was selected, then "left top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -height / 2f, 0.01f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.x < points[1].position.x)
            {
                // first "left bottom" was selected, then "right top". FRONT WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -height / 2f, 0.01f);
            }
        }
        else
        {
            // points[0].normal.z != 0 && points[0].normal.z > 0
            // back
            width = Mathf.Abs(points[0].position.x - points[1].position.x);
            height = Mathf.Abs(points[0].position.y - points[1].position.y);
            //Debug.Log($"x: {width}, y: {height}");
            localRectTransform.sizeDelta = new Vector2(width, height);
            localRectTransform.rotation = Quaternion.Euler(0f, 180f, 0f);

            if (points[0].position.y > points[1].position.y && points[0].position.x > points[1].position.x)
            {
                // first "right top" was selected, then "left bottom". BACK WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, height / 2f, -0.01f);
            }
            else if (points[0].position.y > points[1].position.y && points[0].position.x < points[1].position.x)
            {
                // first "left top" was selected, then "right bottom". BACK WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, height / 2f, -0.01f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.x > points[1].position.x)
            {
                // first "right bottom" was selected, then "left top". BACK WALL
                localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -height / 2f, -0.01f);
            }
            else if (points[0].position.y < points[1].position.y && points[0].position.x < points[1].position.x)
            {
                // first "left bottom" was selected, then "right top". BACK WALL
                localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -height / 2f, -0.01f);
            }

        }

        selectedCanvas = new SelectedCanvas();
        selectedCanvas.normal = points[0].normal;
        selectedCanvas.transform = localCanvas.transform;
        selectedCanvas.width = width;
        selectedCanvas.height = height;

        

        switch (experimentManager.GetExperimentCondition)
        {
            case ExperimentManager.Condition.DirectSketching:
                gazeReader.ChangeState(GazeReader.InteractionState.DrawingOnCanvas);
                break;
            case ExperimentManager.Condition.IndirectSketching:
                Vector3[] bottomCorners = new Vector3[4];
                localRectTransform.GetWorldCorners(bottomCorners);
                Vector3[] topCorners = new Vector3[4];
                for (int i = 0; i < bottomCorners.Length; i++)
                {
                    topCorners[i] = bottomCorners[i] + points[0].normal * 0.25f;
                    //Debug.Log($"top corner: {topCorners[i]} - bottomCorner: {bottomCorners[i]}");
                }

                float minX = Mathf.Infinity;
                float maxX = -Mathf.Infinity;
                float minY = Mathf.Infinity;
                float maxY = -Mathf.Infinity;
                float minZ = Mathf.Infinity;
                float maxZ = -Mathf.Infinity;
                List<Vector3> cornerList = new List<Vector3>();
                foreach (Vector3 corner in bottomCorners)
                {
                    cornerList.Add(corner);
                }
                foreach (Vector3 corner in topCorners)
                {
                    cornerList.Add(corner);
                }
                List<GameObject> cornerSphereList = new List<GameObject>();
                Vector3 posSum = Vector3.zero;
                foreach (Vector3 corner in cornerList)
                {
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

                    GameObject t = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    t.transform.localScale = Vector3.one * 0.05f;
                    t.transform.position = corner;
                    posSum += t.transform.position;
                    cornerSphereList.Add(t);
                }
                selectedArea.minX = minX; selectedArea.maxX = maxX; selectedArea.minY = minY; selectedArea.maxY = maxY; selectedArea.minZ = minZ; selectedArea.maxZ = maxZ;
                //Debug.Log($"X range between: {minX} and {maxX} \n Y range between: {minY} and {maxY} \n Z range between: {minZ} and {maxZ}");
                targetAreaObj = new GameObject("area");
                targetAreaObj.transform.position = posSum / 8f;
                foreach (GameObject obj in cornerSphereList)
                {
                    obj.transform.parent = targetAreaObj.transform;
                }

                wimObj = Instantiate(targetAreaObj, Camera.main.transform.position + Camera.main.transform.forward * 0.5f - Camera.main.transform.up * 0.2f, Quaternion.identity);
                MeshRenderer[] mrs = wimObj.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in mrs)
                {
                    mr.material.color = Color.blue;
                }
                
                targetArea = targetAreaObj.AddComponent<GeneratedArea>();
                targetArea.DesiredPos = targetArea.transform.position;
                wim = wimObj.AddComponent<GeneratedArea>();
                wim.DesiredPos = wim.transform.position;
                wim.transform.localScale = Vector3.one * 0.4f;

                // setting parameters
                targetArea.Brush = brush.transform;
                wim.Brush = brush.transform;
                targetArea.ControlledBy = wim;

                wim.TargetController = brush.GetController;
                wim.controlledByUser = true;     
                
                // spawning control objects
                targetArea.GenerateControl();
                wim.GenerateControl();
                drawController.targetTransformForLineRendererDrawing = wim.GetControl.transform;


                gazeReader.ChangeState(GazeReader.InteractionState.DrawingOnTargetArea);
                break;
        }
        points = new Point[2];
    }

    public void ClearIndirectSketching()
    {
        if(localCanvas != null) Destroy(localCanvas.gameObject);
        Destroy(targetAreaObj);
        Destroy(wimObj);
        localCanvas = null;
        targetAreaObj = null;
        wimObj = null;
    }
    private void CreateBox2D()
    {
        //Debug.Log($"CREATING BOX...");
        Canvas localCanvas = Instantiate(exampleImage);
        localCanvas.renderMode = RenderMode.WorldSpace;
        //Debug.Log($"canvas name: {localCanvas.name}");
        RectTransform localRectTransform = localCanvas.GetComponent<RectTransform>();
        localRectTransform.position = Vector3.zero;
        localRectTransform.sizeDelta = new Vector2(1f, 1f);
        Image localImage = localCanvas.GetComponentInChildren<Image>();
        //localImage.color = Color.blue;
        localRectTransform.anchoredPosition3D = points[0].position;
        //Debug.Log($"CREATED CANVAS ANCHORED POSITION: {localRectTransform.anchoredPosition3D}");

        float width = 0;
        float height = 0;
        
        if (points[0].targetObject == points[1].targetObject)
        {
            switch(points[0].direction)
            {
                case PDirection.Front:
                    width = Mathf.Abs(points[0].position.x - points[1].position.x);
                    height = Mathf.Abs(points[0].position.y - points[1].position.y);
                    //Debug.Log($"x: {width}, y: {height}");
                    localRectTransform.sizeDelta = new Vector2(width, height);

                    if (points[0].position.y > points[1].position.y && points[0].position.x > points[1].position.x)
                    {
                        // first "right top" was selected, then "left bottom". FRONT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, height / 2f, 0.01f);
                    }
                    else if (points[0].position.y > points[1].position.y && points[0].position.x < points[1].position.x)
                    {
                        // first "left top" was selected, then "right bottom". FRONT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, height / 2f, 0.01f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.x > points[1].position.x)
                    {
                        // first "right bottom" was selected, then "left top". FRONT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -height / 2f, 0.01f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.x < points[1].position.x)
                    {
                        // first "left bottom" was selected, then "right top". FRONT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -height / 2f, 0.01f);
                    }
                    break;
                case PDirection.Right:
                    width = Mathf.Abs(points[0].position.z - points[1].position.z);
                    height = Mathf.Abs(points[0].position.y - points[1].position.y);
                    //Debug.Log($"x: {width}, y: {height}");
                    localRectTransform.sizeDelta = new Vector2(width, height);
                    localRectTransform.rotation = Quaternion.Euler(0, 90, 0);

                    if (points[0].position.y > points[1].position.y && points[0].position.z > points[1].position.z)
                    {
                        // first "right top" was selected, then "left bottom". RIGHT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(0.01f, height / 2f, width / 2f);
                    }
                    else if (points[0].position.y > points[1].position.y && points[0].position.z < points[1].position.z)
                    {
                        // first "left top" was selected, then "right bottom". RIGHT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(0.01f, height / 2f, -width / 2f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.z > points[1].position.z)
                    {
                        // first "right bottom" was selected, then "left top". RIGHT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(0.01f, -height / 2f, width / 2f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.z < points[1].position.z)
                    {
                        // first "left bottom" was selected, then "right top". RIGHT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(0.01f, -height / 2f, -width / 2f);
                    }
                    break;
                case PDirection.Left:
                    width = Mathf.Abs(points[0].position.z - points[1].position.z);
                    height = Mathf.Abs(points[0].position.y - points[1].position.y);
                    //Debug.Log($"x: {width}, y: {height}");
                    localRectTransform.sizeDelta = new Vector2(width, height);
                    localRectTransform.rotation = Quaternion.Euler(0, -90, 0);

                    if (points[0].position.y > points[1].position.y && points[0].position.z > points[1].position.z)
                    {
                        // first "right top" was selected, then "left bottom". LEFT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, height / 2f, width / 2f);
                    }
                    else if (points[0].position.y > points[1].position.y && points[0].position.z < points[1].position.z)
                    {
                        // first "left top" was selected, then "right bottom". LEFT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, height / 2f, -width / 2f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.z > points[1].position.z)
                    {
                        // first "right bottom" was selected, then "left top". LEFT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, -height / 2f, width / 2f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.z < points[1].position.z)
                    {
                        // first "left bottom" was selected, then "right top". LEFT WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-0.01f, -height / 2f, -width / 2f);
                    }
                    break;
                case PDirection.Back:
                    width = Mathf.Abs(points[0].position.x - points[1].position.x);
                    height = Mathf.Abs(points[0].position.y - points[1].position.y);
                    //Debug.Log($"x: {width}, y: {height}");
                    localRectTransform.sizeDelta = new Vector2(width, height);
                    localRectTransform.rotation = Quaternion.Euler(0f, 180f, 0f);

                    if (points[0].position.y > points[1].position.y && points[0].position.x > points[1].position.x)
                    {
                        // first "right top" was selected, then "left bottom". BACK WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, height / 2f, -0.01f);
                    }
                    else if (points[0].position.y > points[1].position.y && points[0].position.x < points[1].position.x)
                    {
                        // first "left top" was selected, then "right bottom". BACK WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, height / 2f, -0.01f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.x > points[1].position.x)
                    {
                        // first "right bottom" was selected, then "left top". BACK WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(width / 2f, -height / 2f, -0.01f);
                    }
                    else if (points[0].position.y < points[1].position.y && points[0].position.x < points[1].position.x)
                    {
                        // first "left bottom" was selected, then "right top". BACK WALL
                        localRectTransform.anchoredPosition3D -= new Vector3(-width / 2f, -height / 2f, -0.01f);
                    }
                    break;
            }
        }
        else
        {
            points = new Point[2];
            return;
        }
        
 


        points = new Point[2];

    }

    private void Creating3D_Rectangle()
    {
        Mesh mesh = new Mesh();

    }
}
