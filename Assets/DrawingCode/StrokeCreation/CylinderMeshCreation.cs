/* 
 * mayra barrera, 2019
 * a class that renders a cylinder based on the user hand position
 * based on Jasper Flick, Swirly Pipe tutorial (https://catlikecoding.com/unity/tutorials/swirly-pipe/)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CylinderMeshCreation : MonoBehaviour {

    private int _orientation; //0-lateral, 1-

    private string _meshName;
    private float _meshWidth; //radius

    private int _meshRingCount;
    private int _meshTopCount;
    private int _lastRingFirstIndex;
    
    //public Shader _meshShader;
    public Material _meshMaterial;

    private Mesh _mesh;
   
    private int _currentStage;
    private Vector3 _lastPosition;

    //!!TODO!!
    //missing normals
    
    public void NewMesh(string name, float size, int ringCount, Color color, Material selectedMaterial)
    {
        _meshName = name;
        _meshWidth = size;

        _meshRingCount = ringCount;
        _meshTopCount = ringCount * 3;
        _lastRingFirstIndex = 3;

        //_meshMaterial = new Material(_meshShader);
        //_meshMaterial.SetColor("_Color", color);

        _mesh = new Mesh();
        _mesh.name = _meshName;
        GetComponent<MeshFilter>().mesh = _mesh;
        MeshCollider mCollider = GetComponent<MeshCollider>();
        mCollider.sharedMesh = _mesh;
        GetComponent<MeshRenderer>().material = selectedMaterial; // change this material to change the color for that stroke



        _currentStage = 1;
        _lastPosition = Vector3.zero;
    }

    public bool CreateCylinderMesh(Vector3 vertex, Matrix4x4 directionPose)
    {
        bool strokeExists = false;

        if (_lastPosition != Vector3.zero)
        {
            AddMeshFilter(CreateQuads(_lastPosition, vertex, directionPose, _currentStage));

            strokeExists = true;
            _currentStage = 2;
        }

        _lastPosition = vertex;
        return strokeExists;
    }

    public void CreateLastCylinderMesh(Vector3 finalVertex, Matrix4x4 directionPose, int newStage)
    {
        _currentStage = newStage;

        AddMeshFilter(CreateQuads(_lastPosition, finalVertex, directionPose, _currentStage));
        MeshCollider mCollider = GetComponent<MeshCollider>();
        mCollider.enabled = true;
        // not make it convex. if you make it convex, you can not select strokes that are behind of circular strokes (the one at the front prevents selection)
        //mCollider.convex = true;

    }

    #region mesh methods
    public Vector3[] CreateQuads(Vector3 lastPos, Vector3 currentPos, Matrix4x4 directionPose, int state)
    {
        Vector3[] qs;

        //number of degrees between each ring_vertex based on the number of vertex in each ring
        float arcAngle = (2f * Mathf.PI) / _meshRingCount;

        float actualDegrees = 0;
        float actualDegreesDouble = arcAngle;
        int vertexNumber = 0;

        //many ways to crate a cylinder, I decided to use projection matrix because FUN!!! but this always needs to be 0
        float curveAngle = 0;

        switch (state)
        {
            #region start
            case 1:            
                
                vertexNumber = _meshRingCount * 4 + 1;//vertex in ring * number of vertex of a quad + start vertex
                qs = new Vector3[vertexNumber];

                qs[0] = lastPos;

                for (int i = 1; i < qs.Length; i += 4)
                {
                    qs[i] = lastPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegrees));
                    qs[i + 1] = lastPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegreesDouble));

                    qs[i + 2] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegrees));
                    qs[i + 3] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegreesDouble));

                    actualDegrees += arcAngle;
                    actualDegreesDouble += arcAngle;
                }
                break;
            #endregion

            #region normal
            case 2:

                vertexNumber = _meshRingCount * 2; //vertex in ring * second half vertex of quad
                qs = new Vector3[vertexNumber];

                for (int i = 0; i < qs.Length; i += 2)
                {
                    qs[i] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegrees));
                    qs[i + 1] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegreesDouble));

                    actualDegrees += arcAngle;
                    actualDegreesDouble += arcAngle;
                }
                break;
            #endregion

            #region end
            case 3://end

                vertexNumber = _meshRingCount * 2 + 1;//vertex in ring * second half vertex of quad + end vertex
                qs = new Vector3[vertexNumber];

                for (int i = 0; i < qs.Length-1; i += 2)
                {
                    qs[i] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegrees));
                    qs[i + 1] = currentPos + directionPose.MultiplyPoint3x4(GetPointOnTorus(curveAngle, actualDegreesDouble));

                    actualDegrees += arcAngle;
                    actualDegreesDouble += arcAngle;
                }

                qs[qs.Length - 1] = currentPos;

                break;
            #endregion

            default:
                Debug.Log("something went wrong in mesh creation");
                qs = new Vector3[0];
                break;
        }

        return qs;
    }

    private Vector3 GetPointOnTorus(float curveAngle, float pipeAngle)
    {
        Vector3 p;
        float r = (_meshWidth * Mathf.Cos(pipeAngle));
        p.x = r * Mathf.Sin(curveAngle);
        p.y = r * Mathf.Cos(curveAngle);
        p.z = _meshWidth * Mathf.Sin(pipeAngle);

        return p;
    }

    public void AddMeshFilter(Vector3[] quads)
    {

        #region vertices
        int vL = _mesh.vertices.Length;

        Vector3[] _vertices = _mesh.vertices;
        _vertices = ResizeV3(_vertices, quads.Length);

        for (int i = 0; i < quads.Length; i++)
        {
            _vertices[vL + i] = quads[i];
        }
        #endregion

        #region UVs
        int uL = _mesh.uv.Length;

        Vector2[] _uv = _mesh.uv;

        _uv = ResizeV2(_uv, quads.Length);

        int _vert;
        int _sideCounter;

        switch (_currentStage)
        {
            #region start
            case 1:

                _vert = 0;

                ////top
                _uv[_vert++] = new Vector2(0f, 0f);

                //sides
                _sideCounter = 0;
                while (_vert < _uv.Length)
                {
                    float t = _sideCounter++ / _meshRingCount;
                    _uv[_vert++] = new Vector2(t, 0f);
                    _uv[_vert++] = new Vector2(t, 1f);
                }
                break;
            #endregion

            #region normal
            case 2:

                _vert = uL;

                //side
                _sideCounter = 0;
                while (_vert < _uv.Length)
                {
                    float t = _sideCounter++ / _meshRingCount;
                    _uv[_vert++] = new Vector2(t, 0f);
                    _uv[_vert++] = new Vector2(t, 1f);
                }

                break;
            #endregion

            #region end
            case 3:

                _vert = uL;

                //side
                _sideCounter = 0;
                while (_vert < _uv.Length - 1)
                {
                    float t = _sideCounter++ / _meshRingCount;
                    _uv[_vert++] = new Vector2(t, 0f);
                    _uv[_vert++] = new Vector2(t, 1f);
                }

                //botom
                _uv[_vert++] = new Vector2(1f, 1f);

                break;
            #endregion

            default:
                Debug.Log("error wrong stage");
                break;
        }
        #endregion

        #region triangles
        int tL = _mesh.triangles.Length;
        int[] _triangles = _mesh.triangles;

        //first/last segment, add top triangles, if not only add number of vertex necessary to create a quad every 4 vertex, then multiply by 2 to get back/front triangles
        //(number of vertex * 2 (two triangles each quad)) * 3 (number of vertex in each triangle)
        int triangleFaces = _meshRingCount * 6;
        int doubleMeshTop = _meshTopCount * 2;

        int newTriangles = _currentStage == 2 ? triangleFaces : triangleFaces + _meshTopCount;
        _triangles = ResizeInt(_triangles, newTriangles * 2);

        //first index of previous ring
        int firstRing = _lastRingFirstIndex;

        //first index of current ring
        int secondRing = vL;

        //number of front face triangles
        int halfNewFace = tL + triangleFaces;

        //create mesh
        switch (_currentStage)
        {
            #region start
            case 1:
           
                int halfTrianglesLength = triangleFaces + doubleMeshTop;

                //top front
                for (int t = 0, i = 1; t < _meshTopCount; t += 3, i += 4)
                {
                    _triangles[t] = 0;
                    _triangles[t + 1] = i + 1;
                    _triangles[t + 2] = i;
                }

                //top back
                for (int t = _meshTopCount, i = 1; t < doubleMeshTop; t += 3, i += 4)
                {
                    _triangles[t] = 0;
                    _triangles[t + 1] = i;
                    _triangles[t + 2] = i + 1;
                }

                //sides front
                for (int t = doubleMeshTop, i = 1; t < halfTrianglesLength; t += 6, i += 4)
                {
                    int vertexIndex = vL + i;

                    _triangles[t] = vertexIndex;
                    _triangles[t + 1] = vertexIndex + 1;
                    _triangles[t + 2] = vertexIndex + 2;

                    _triangles[t + 3] = vertexIndex + 2;
                    _triangles[t + 4] = vertexIndex + 1;
                    _triangles[t + 5] = vertexIndex + 3;
                }

                //sides back
                for (int t = halfTrianglesLength, i = 1; t < _triangles.Length; t += 6, i += 4)
                {
                    int vertexIndex = vL + i;

                    _triangles[t] = vertexIndex;
                    _triangles[t + 1] = vertexIndex + 2;
                    _triangles[t + 2] = vertexIndex + 1;

                    _triangles[t + 3] = vertexIndex + 2;
                    _triangles[t + 4] = vertexIndex + 3;
                    _triangles[t + 5] = vertexIndex + 1;
                }

                break;
            #endregion

            #region normal
            case 2:

                //sides front
                for (int t = tL; t < halfNewFace; t += 6)
                {
                    #region test
                    /*
                    int temp = t;
                    int tempF = firstRing;
                    int tempS = secondRing;

                    Debug.Log(temp + "   " + tempF);

                    temp++;                  
                    Debug.Log(temp + "   " + tempS);

                    temp++;
                    tempF++;
                    Debug.Log(temp + "   " + tempF);

                    temp++;
                    Debug.Log(temp + "   " + tempS);

                    temp++;
                    Debug.Log(temp + "   " + tempF);

                    temp++;
                    tempS++;
                    Debug.Log(temp + "   " + tempS);
                    */
                    #endregion

                    _triangles[t] = firstRing;
                    _triangles[t + 1] = firstRing + 1;
                    _triangles[t + 2] = secondRing;

                    _triangles[t + 3] = secondRing;
                    _triangles[t + 4] = firstRing + 1;
                    _triangles[t + 5] = secondRing + 1;

                    if (_lastRingFirstIndex == 3)
                    {
                        firstRing += 4;
                    }
                    else
                    {
                        firstRing += 2;
                    }
                    secondRing += 2;
                }

                firstRing = _lastRingFirstIndex;
                secondRing = vL;

                for (int t = halfNewFace; t < _triangles.Length; t += 6)
                {
                    #region test
                    /*
                    int temp = t;
                    int tempF = firstRing;
                    int tempS = secondRing;

                    Debug.Log(temp + "   " + tempF);

                    temp++;                  
                    Debug.Log(temp + "   " + tempS);

                    temp++;
                    tempF++;
                    Debug.Log(temp + "   " + tempF);

                    temp++;
                    Debug.Log(temp + "   " + tempS);

                    temp++;
                    Debug.Log(temp + "   " + tempF);

                    temp++;
                    tempS++;
                    Debug.Log(temp + "   " + tempS);
                    */
                    #endregion

                    _triangles[t] = firstRing;
                    _triangles[t + 1] = secondRing;
                    _triangles[t + 2] = firstRing + 1;

                    _triangles[t + 3] = secondRing;
                    _triangles[t + 4] = secondRing + 1;
                    _triangles[t + 5] = firstRing + 1;

                    if (_lastRingFirstIndex == 3)
                    {
                        firstRing += 4;
                    }
                    else
                    {
                        firstRing += 2;
                    }
                    secondRing += 2;
                }

                _lastRingFirstIndex = vL;

                break;

            #endregion

            #region end
            case 3:

                int vertexSides = _triangles.Length - doubleMeshTop;
                int faceSide = _triangles.Length - _meshTopCount;

                //side front
                for (int t = tL; t < halfNewFace; t += 6)
                {
                    _triangles[t] = firstRing;
                    _triangles[t + 1] = firstRing + 1;
                    _triangles[t + 2] = secondRing;

                    _triangles[t + 3] = secondRing;
                    _triangles[t + 4] = firstRing + 1;
                    _triangles[t + 5] = secondRing + 1;

                    firstRing += 2;
                    secondRing += 2;
                }

                firstRing = _lastRingFirstIndex;
                secondRing = vL;

                //side back
                for (int t = halfNewFace; t < vertexSides; t += 6)
                {
                    _triangles[t] = firstRing;
                    _triangles[t + 1] = secondRing;
                    _triangles[t + 2] = firstRing + 1;

                    _triangles[t + 3] = secondRing;
                    _triangles[t + 4] = secondRing + 1;
                    _triangles[t + 5] = firstRing + 1;

                    firstRing += 2;
                    secondRing += 2;
                }

                //end front
                secondRing = vL;
                for (int t = vertexSides; t < faceSide; t += 3)
                {
                    _triangles[t] = _vertices.Length - 1;
                    _triangles[t + 1] = secondRing;
                    _triangles[t + 2] = secondRing + 1;

                    secondRing += 2;
                }

                //end back
                secondRing = vL;
                for (int t = faceSide; t < _triangles.Length; t += 3)
                {
                    _triangles[t] = _vertices.Length - 1;
                    _triangles[t + 1] = secondRing + 1;
                    _triangles[t + 2] = secondRing;

                    secondRing += 2;
                }

                break;

            #endregion

            default:
                Debug.Log("error wrong stage");
                break;
        }
        #endregion

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
    }

    #endregion

    #region resizeArrays
    private Vector4[] ResizeV4(Vector4[] oldArray, int newLength)
    {
        Vector4[] newArray = new Vector4[oldArray.Length + newLength];

        for (int i = 0; i < oldArray.Length; i++)
        {
            newArray[i] = oldArray[i];
        }

        return newArray;
    }

    private Vector3[] ResizeV3(Vector3[] oldArray, int newLength)
    {
        Vector3[] newArray = new Vector3[oldArray.Length + newLength];

        for (int i = 0; i < oldArray.Length; i++)
        {
            newArray[i] = oldArray[i];
        }

        return newArray;
    }

    private Vector2[] ResizeV2(Vector2[] oldArray, int newLength)
    {
        Vector2[] newArray = new Vector2[oldArray.Length + newLength];

        for (int i = 0; i < oldArray.Length; i++)
        {
            newArray[i] = oldArray[i];
        }

        return newArray;
    }

    private int[] ResizeInt(int[] oldArray, int newLength)
    {
        int[] newArray = new int[oldArray.Length + newLength];

        for (int i = 0; i < oldArray.Length; i++)
        {
            newArray[i] = oldArray[i];
        }

        return newArray;
    }
    #endregion
}
