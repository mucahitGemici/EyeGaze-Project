using UnityEngine;
using UnityEngine.UI;
public class ColorButton : MonoBehaviour
{
    public Image outlineImage;

    // this is gonna be activated with raycasting (left controller raycast)
    [SerializeField] public bool isHovering;

    private void Update()
    {
        if (isHovering)
        {
            outlineImage.color = Color.green;
        }
        else
        {
            outlineImage.color = Color.white;
        }
    }
}
