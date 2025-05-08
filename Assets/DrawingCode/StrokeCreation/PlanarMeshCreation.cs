/* mayra barrera, 2017
 * class to create the mesh of a gameobject */

using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
class PlanarMeshCreation : MonoBehaviour
{
    private string _meshName;
    public float _meshWidth;

    public Shader _meshShader;
    private Material _meshMaterial;
    private Mesh _mesh;

    private bool firstQuad;
    private Vector3 _lastPosition;
    //public Vector3 meshLastPosition = Vector3.zero;

    public void NewMesh(string name, float size, Color color)
    {
        _meshName = name;
        _meshWidth = size;

        _meshMaterial = new Material(_meshShader);
        _meshMaterial.SetColor("_Color", color);
        _meshMaterial.SetColor("_OutlineColor", color);
        _meshMaterial.SetFloat("_BorderWidth", 0f);       
        _meshMaterial.SetTexture("_MainTex", Resources.Load("Textures/brush1") as Texture);

        _mesh = new Mesh();
        _mesh.name = _meshName;
        GetComponent<MeshFilter>().mesh = _mesh;
        GetComponent<MeshRenderer>().material = _meshMaterial;

        firstQuad = true;
        _lastPosition = Vector3.zero;
    }

    public bool CreateNormalMesh(Vector3 vertex, Vector3 normal)
    {
        RibbonVertex newRibbonVertix;
        bool exist;

        if (_lastPosition != Vector3.zero)
        {
            Vector3 ribbonDirection = Vector3.Cross(normal, vertex - _lastPosition).normalized;

            AddMeshFilter(NewQuad(_lastPosition, vertex, ribbonDirection, firstQuad));
            newRibbonVertix = new RibbonVertex(_lastPosition, ribbonDirection, normal);

            firstQuad = false;
            exist = true;
        }
        else
        {
            newRibbonVertix = new RibbonVertex();
            exist = false;
        }

        _lastPosition = vertex;
        return exist;
    }

    public void LastCreateNormalMesh(Vector3 lastVertex, Vector3 newVertex, Vector3 normal)
    {
        //add single vertex to stroke
        Vector3 ribbonDirection = Vector3.Cross(normal, newVertex - lastVertex).normalized;
        AddMeshFilter(NewQuad(lastVertex, newVertex, ribbonDirection, firstQuad));
    }

    public void UpdateColorOutline(Color color, float size)
    {
        _meshMaterial.SetColor("_OutlineColor", color);
        _meshMaterial.SetFloat("_BorderWidth", size);
    }

    #region mesh creation
    //create mesh based on quad
    public void AddMeshFilter(Vector3[] quad)
    {
        int vL = _mesh.vertices.Length;
        Vector3[] _vertices = _mesh.vertices;
        _vertices = ResizeV3(_vertices, 2 * quad.Length);

        for (int i = 0; i < 2 * quad.Length; i += 2)
        {
            _vertices[vL + i] = quad[i / 2];
            _vertices[vL + i + 1] = quad[i / 2];
        }

        Vector2[] _uv = _mesh.uv;
        _uv = ResizeV2(_uv, 2 * quad.Length);

        Vector4[] _tangents = _mesh.tangents;
        _tangents = ResizeV4(_tangents, 2 * quad.Length);

        if (quad.Length == 4)
        {
            _uv[vL] = Vector2.zero;
            _uv[vL + 1] = Vector2.zero;
            _uv[vL + 2] = Vector2.right;
            _uv[vL + 3] = Vector2.right;
            _uv[vL + 4] = Vector2.up;
            _uv[vL + 5] = Vector2.up;
            _uv[vL + 6] = Vector2.one;
            _uv[vL + 7] = Vector2.one;
        }
        else
        {
            if (vL % 8 == 0)
            {
                _uv[vL] = Vector2.zero;
                _uv[vL + 1] = Vector2.zero;
                _uv[vL + 2] = Vector2.right;
                _uv[vL + 3] = Vector2.right;
            }
            else
            {
                _uv[vL] = Vector2.up;
                _uv[vL + 1] = Vector2.up;
                _uv[vL + 2] = Vector2.one;
                _uv[vL + 3] = Vector2.one;
            }
        }

        int tL = _mesh.triangles.Length;

        int[] _triangles = _mesh.triangles;
        _triangles = ResizeInt(_triangles, 12);

        if (quad.Length == 2)
        {
            vL -= 4;
        }

        // front-facing quad
        _triangles[tL] = vL;
        _triangles[tL + 1] = vL + 2;
        _triangles[tL + 2] = vL + 4;

        _triangles[tL + 3] = vL + 2;
        _triangles[tL + 4] = vL + 6;
        _triangles[tL + 5] = vL + 4;

        // back-facing quad
        _triangles[tL + 6] = vL + 5;
        _triangles[tL + 7] = vL + 3;
        _triangles[tL + 8] = vL + 1;

        _triangles[tL + 9] = vL + 5;
        _triangles[tL + 10] = vL + 7;
        _triangles[tL + 11] = vL + 3;

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();

        //meshLastPosition = _mesh.vertices[_mesh.vertices.Length - 1];
    }

    //create quad based on vertix
    public Vector3[] NewQuad(Vector3 lastPosition, Vector3 newPosition, Vector3 ribbonDirection, bool first)
    {
        Vector3[] q;
        if (first)
        {
            q = new Vector3[4];
            q[0] = lastPosition + ribbonDirection * _meshWidth / 2; //up
            q[1] = lastPosition + ribbonDirection * _meshWidth / -2; //down
            q[2] = newPosition + ribbonDirection  * _meshWidth / 2;
            q[3] = newPosition + ribbonDirection  * _meshWidth / -2;
        }
        else
        {
            q = new Vector3[2];
            q[0] = lastPosition + ribbonDirection * _meshWidth / 2; //up
            q[1] = lastPosition + ribbonDirection * _meshWidth / -2; //down
        }

        return q;
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

    private Vector3[] AddVertices(Vector3[] vertices, Vector3 newControlPos, Vector3 newDirectionPos, bool tmp)
    {
        if (!tmp || vertices.Length == 0) vertices = Resize(vertices, 2);

        //new control vertex
        int skippedPosition = (vertices.Length / 2) - 1;
        vertices[skippedPosition] = newControlPos;

        //new direction vertex
        vertices[vertices.Length - 1] = newDirectionPos;

        return vertices;
    }

    private Vector3[] Resize(Vector3[] oldVertices, int ns)
    {
        Vector3[] newVertices = new Vector3[oldVertices.Length + ns];

        int skippedPosition = (newVertices.Length / 2) - 1;

        for (int i = 0; i < newVertices.Length - 1; i++)
        {
            if (i != skippedPosition)
            {

                if (i < skippedPosition)
                {
                    newVertices[i] = oldVertices[i];
                }
                else
                {
                    newVertices[i] = oldVertices[i - 1];
                }
            }
        }

        return newVertices;
    }
    #endregion


}

