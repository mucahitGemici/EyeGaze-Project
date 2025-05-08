/* 
 * mayra barrera, 2017
 * script for draw events
 * 
 */

using UnityEngine;

//drawing events
public class StartDrawingEvent : AppEvent
{

    public Transform brush;

    public StartDrawingEvent(Transform _brush)
    {
        brush = _brush;
    }
}

public class EndDrawingEvent : AppEvent
{

    public Transform brush;

    public EndDrawingEvent(Transform _brush)
    {
        brush = _brush;
    }
}

public class OnDrawingEvent : AppEvent
{
    public Transform transform;

    public OnDrawingEvent(Transform _transform)
    {
        transform = _transform;
    }
}

public class DeleteEvent: AppEvent
{
    public bool delete;
    public Vector3 position;
    public Vector3 right;
    public Vector3 up;

    public DeleteEvent(bool _delete, Vector3 _position, Vector3 _righ, Vector3 _up)
    {
        delete = _delete;
        position = _position;
        right = _righ;
        up = _up;
    }
}

public class NewCanvasEvent : AppEvent
{
    public NewCanvasEvent()
    {

    }
}

//camera control events
public class StartRotationEvent : AppEvent
{
    public Transform controller;
    public int controllerName;

    public StartRotationEvent(Transform _controller, int _controllerName)
    {
        controller = _controller;
        controllerName = _controllerName;
    }
}

public class EndRotationEvent : AppEvent
{
    public Transform controller;
    public int controllerName;

    public EndRotationEvent(Transform _controller, int _controllerName)
    {
        controller = _controller;
        controllerName = _controllerName;
    }
}

public class SetLeftControllerGameobjectEvent : AppEvent
{
    public Transform controller;

    public SetLeftControllerGameobjectEvent(Transform _controller)
    {
        controller = _controller;
    }
}

//visual guides events
public class OnChangeColorEvent : AppEvent
{
    public Color color;

    public OnChangeColorEvent(Color _color)
    {
        color = _color;
    }
}