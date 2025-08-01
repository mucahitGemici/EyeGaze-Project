using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SurfaceInteraction : MonoBehaviour
{
    public enum PDirection
    {
        Right,
        Left,
        Front,
        Back
    }
    public struct Point
    {
        public Vector3 position;
        public GameObject targetObject;
        public PDirection direction;
    }
    private Point[] points = new Point[2];

    [SerializeField] private Canvas exampleImage;
    [SerializeField] private GameObject pointPrefab;

    private GameObject pointHolder;
    public void AddPoint(Point pt)
    {
        // returns if the operation is successful or not
        // if we try to add more points than 2, it will return false

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

        return;

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
}
