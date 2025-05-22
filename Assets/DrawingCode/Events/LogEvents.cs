/* 
 * mayra barrera, 2017
 * script for log events
 * 
 */

using UnityEngine;

//visual widgets - ray
public class CreateRayWidgetEvent : AppEvent
{
    public int name;
    public Vector3 startPoint;
    public Color color;

    public Vector3 direction;
    public float size;

    public CreateRayWidgetEvent(int _name, Vector3 _startPoint, Color _color, Vector3 _direction, float _size)
    {
        name = _name;
        startPoint = _startPoint;
        color = _color;
        direction = _direction;
        size = _size;
    }
}

//visual widgets - point
public class CreatePointWidgetEvent : AppEvent
{
    public int name;
    public Vector3 startPoint;
    public Color color;
    public float size;
    public bool specialType;

    public CreatePointWidgetEvent(int _name, Vector3 _startPoint, Color _color, float _size, bool _specialType = false)
    {
        name = _name;
        startPoint = _startPoint;
        color = _color;
        size = _size;
        specialType = _specialType;
    }
}

//visual widget - text
public class CreateTextWidgetEvent : AppEvent
{
    public int name;
    public Vector3 position;
    public Color color;
    public float size;
    public string text;

    public CreateTextWidgetEvent(int _name, Vector3 _position, Color _color, float _size, string _text)
    {
        name = _name;
        position = _position;
        color = _color;
        size = _size;
        text = _text;
    }
}

//update widget
public class UpdateWidgetEvent : AppEvent
{
    public int name;
    public Vector3 position;
    public Vector3 direction;
    public float size;

    public UpdateWidgetEvent(int _name, Vector3 _position, Vector3 _direction, float _size)
    {
        name = _name;
        position = _position;
        direction = _direction;
        size = _size;
    }
}

//delete a visual widget
public class DeleteVisualWidgetEvent : AppEvent
{
    public int name;

    public DeleteVisualWidgetEvent(int _name)
    {
        name = _name;
    }
}

public class DeleteAllWidgetsEvent : AppEvent
{
    public DeleteAllWidgetsEvent()
    {

    }
}

//create a xml doc
public class CreateXMLEvent : AppEvent
{
    public bool specificName;
    public string name;
    public string participantName;

    public CreateXMLEvent(bool _specificName, string _name, string _participantName)
    {
        specificName = _specificName;
        name = _name;
        participantName = _participantName;
    }
}

//write log in xml
public class CreateEntryEvent: AppEvent
{
    public string participantName;
    public string[] experimentVariables;

    public GlobalVars.LogCategory category;
    public string value;

    public CreateEntryEvent(string _participantName, string[] _experimentVariables, GlobalVars.LogCategory _category, string _value)
    {
        participantName = _participantName;

        experimentVariables = new string[_experimentVariables.Length];
        for(int i=0; i<_experimentVariables.Length; i++)
        {
            experimentVariables[i] = _experimentVariables[i];
        }

        category = _category;
        value = _value;
    }
}

//close xml
public class EndLogEvent: AppEvent
{
    public EndLogEvent()
    {

    }
}

//create screenshot
public class TakeScreenshotEvent : AppEvent
{
    public string participantName;
    public int experiment;

    public string[] experimentVariables;

    public string CameraName;
    public int WidthResolution;
    public int HeightResolution;

    public TakeScreenshotEvent(string _participantName, int _experiment, string[] _experimentVariables, string _cameraName, int _widthResolution, int _heightResolution)
    {
        participantName = _participantName;
        experiment = _experiment;

        experimentVariables = new string[_experimentVariables.Length];
        for (int i = 0; i < _experimentVariables.Length; i++)
        {
            experimentVariables[i] = _experimentVariables[i];
        }

        CameraName = _cameraName;
        WidthResolution = _widthResolution;
        HeightResolution = _heightResolution;
    }
}

//save meshFilter for later viewing
public class SaveStrokeEvent : AppEvent
{

    public string participantName;

    public string[] experimentVariables;

    public string meshName;
    public Mesh mesh;
    public Material material;

    public SaveStrokeEvent(string _participantName, string[] _experimentVariables, string _meshName, Mesh _mesh, Material _material)
    {
        participantName = _participantName;

        experimentVariables = new string[_experimentVariables.Length];
        for (int i = 0; i < _experimentVariables.Length; i++)
        {
            experimentVariables[i] = _experimentVariables[i];
        }

        meshName = _meshName;
        mesh = _mesh;
        material = _material;
    }
}

