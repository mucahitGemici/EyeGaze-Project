/* 
 * mayra barrera, 2017
 * a vertex for the ribbon
 */

using UnityEngine;

public class LogPosition {

    private Vector3 _position;
    private Vector3 _viewDirection;
    private string _time;

    private Vector3 _directionPreviousVertex;
    private float _distancePreviousVertex;
   
    private int _group;

    public LogPosition()
    {
        Position = Vector3.zero;
        ViewDirection = Vector3.zero;
        Time = "";

        DirectionPreviousVertex = Vector3.zero;
        DistancePreviousVertex = 0;
        
        Group = -1;
    }

    public LogPosition(Vector3 position, Vector3 viewDirection, string time, Vector3 directionPV, float distancePV, int group)
    {
        Position = position;
        ViewDirection = viewDirection;
        Time = time;

        DirectionPreviousVertex = viewDirection;
        DistancePreviousVertex = distancePV;

        Group = group;
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
        }
    }

    public Vector3 ViewDirection
    {
        get
        {
            return _viewDirection;
        }

        set
        {
            _viewDirection = value;
        }
    }

    public Vector3 DirectionPreviousVertex
    {
        get
        {
            return _directionPreviousVertex;
        }

        set
        {
            _directionPreviousVertex = value;
        }
    }

    public float DistancePreviousVertex
    {
        get
        {
            return _distancePreviousVertex;
        }

        set
        {
            _distancePreviousVertex = value;
        }
    }

    public string Time
    {
        get
        {
            return _time;
        }

        set
        {
            _time = value;
        }
    }

    public int Group
    {
        get
        {
            return _group;
        }

        set
        {
            _group = value;
        }
    }
}
