/* 
 * mayra barrera, 2017
 * a vertex for the ribbon surface
 */

using UnityEngine;

public class RibbonVertex {

    private Vector3 _position;
    private Vector3 _ribbonDirection;
    private Vector3 _ribbonNormal;

    public RibbonVertex()
    {
        _position = Vector3.zero;
        _ribbonDirection = Vector3.zero;
        _ribbonNormal = Vector3.zero;
    }

    public RibbonVertex(Vector3 position, Vector3 ribbonDirection, Vector3 surfaceNormal)
    {
        _position = position;
        _ribbonDirection = ribbonDirection;
        _ribbonNormal = surfaceNormal;
    }

    public Vector3 position
    {
        get { return _position; }
        set { _position = value; }
    }

    public Vector3 ribbonDirection
    {
        get { return _ribbonDirection; }
        set { _ribbonDirection = value; }
    }

    public Vector3 ribbonNormal
    {
        get { return _ribbonNormal; }
        set { _ribbonNormal = value; }
    }
}
