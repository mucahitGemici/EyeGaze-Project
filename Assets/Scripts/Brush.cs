using TMPro;
using UnityEngine;

public class Brush : MonoBehaviour
{
    [SerializeField] private TMP_Text testText;

    public bool isDrawingOnArea;

    public bool directSketchingStarted;
    
    public struct DirectSketchingVariables
    {
        public Vector3 normalVector;
        public Vector3 originPoint;
        public float width;
        public float height;
    }
    float s = 1f;
    private DirectSketchingVariables directSketchingVariables;
    public DirectSketchingVariables SetDirectSketchingVariables
    {
        set 
        { 
            directSketchingVariables = value;
            
        }
    }

    [SerializeField] private ExperimentManager experimentManager;
    [SerializeField] private Transform brushVisual;

    //public Transform WIMTransform;
    //public Transform TargetAreaTransform;

    private Vector3 positionOffset;
    public Vector3 PositionOffset
    {
        get { return positionOffset; }
        set { positionOffset = value; }
    }


    [SerializeField] private Transform _targetController;
    public Transform GetController
    {
        get { return _targetController; }
    }
    [SerializeField] private DrawController _drawController;

    private MeshRenderer _meshRenderer;
    [SerializeField] Material _readyMaterial;
    [SerializeField] Material _usingMaterial;

    public enum BrushState
    {
        Ready,
        Using
    }

    private BrushState _state;
    public BrushState SetBrushState
    {
        set { _state = value; }
    }

    [SerializeField] private PositionManipulator _positionManipulator;
    [SerializeField] private ScaleManipulator _scaleManipulator;
    [SerializeField] private RotationManipulator _rotationManipulator;
    private void Start()
    {
        _meshRenderer = _targetController.GetComponent<MeshRenderer>();
        switch (experimentManager.GetExperimentCondition)
        {
            case ExperimentManager.Condition.DirectSketching:
                break;
            case ExperimentManager.Condition.IndirectSketching:
                //
                //brushVisual.gameObject.SetActive(false);
                break;
        }
    }

    public float CalculateS()
    {
        s = Vector3.Distance(_targetController.position, directSketchingVariables.originPoint);
        return s;
    }
    private void Update()
    {

        if(experimentManager.GetExperimentCondition == ExperimentManager.Condition.DirectSketching)
        {
            if (directSketchingStarted)
            {
                Debug.Log($"s: {s}");
                Vector3 realNewPos = _targetController.position * s + positionOffset;


                Vector3 reflectedOnPlane = LimitMovementForDirectSketching(realNewPos);

                transform.position = reflectedOnPlane;
            }
            else
            {
                transform.position = _targetController.position;
            }          
        }
        else
        {
            transform.position = _targetController.position;
        }



        transform.rotation = _targetController.rotation;

        if(_state == BrushState.Ready)
        {
            _meshRenderer.material = _readyMaterial;
        }
        else
        {
            _meshRenderer.material = _usingMaterial;
        }

    }

    private Vector3 LimitMovementForDirectSketching(Vector3 pos)
    {
        float dist = Vector3.Dot(pos - directSketchingVariables.originPoint, directSketchingVariables.normalVector);
        if(dist > 0f)
        {
            return pos;
        }
        else
        {
            return pos - dist * directSketchingVariables.normalVector;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            _positionManipulator.IsHoveringHolder = true;
        }
        else if(other.gameObject.layer == 13)
        {
            _scaleManipulator.IsHoveringHolder = true;
        }
        else if(other.gameObject.layer == 14)
        {
            _rotationManipulator.initialRot = _targetController.rotation;
            _rotationManipulator.IsHoveringHolder = true;
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            _positionManipulator.IsHoveringHolder = false;
        }
        else if(other.gameObject.layer == 13)
        {
            _scaleManipulator.IsHoveringHolder = false;
        }
        else if(other.gameObject.layer == 14)
        {
            _rotationManipulator.IsHoveringHolder = false;
        }
    }


}
