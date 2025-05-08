using UnityEngine;

public class GazeFocus : MonoBehaviour
{
    private float counter;
    private bool isFocused;
    [SerializeField] private MeshRenderer meshRenderer;
    private Color initialColor;

    private void Awake()
    {
        
    }
    private void Start()
    {
        initialColor = meshRenderer.material.color;
    }

    private void Update()
    {
        if (counter >= 0 && isFocused == true)
        {
            meshRenderer.material.color = Color.green;
        }


        counter -= Time.deltaTime;
        if(counter < 0 && isFocused == true)
        {
            isFocused = false;
            meshRenderer.material.color = initialColor;
        }
    }
    public void FocusWithGaze()
    {
        counter = 1f;
        isFocused = true;
    }
}
