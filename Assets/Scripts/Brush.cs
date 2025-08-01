using UnityEngine;

public class Brush : MonoBehaviour
{
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
    }
    private void Update()
    {
        transform.position = _targetController.position;
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
