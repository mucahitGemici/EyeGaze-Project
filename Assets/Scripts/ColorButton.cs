using UnityEngine;
using UnityEngine.UI;
public class ColorButton : MonoBehaviour
{
    public Image outlineImage;

    // this is gonna be activated with raycasting (left controller raycast)
    [SerializeField] public bool isHovering;

    enum ButtonColor
    {
        Black,
        Red
    }

    [SerializeField] private ButtonColor buttonColor;

    private float counter;

    private void Update()
    {
        if (isHovering && counter > 0f)
        {
            outlineImage.color = Color.green;
        }
        else
        {
            outlineImage.color = Color.white;
        }

        counter -= Time.deltaTime;
        if(counter <= 0 && isHovering == true)
        {
            outlineImage.color = Color.white;
            isHovering = false;
        }

        bool buttonIsPressing = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RHand);
        if (buttonIsPressing && isHovering)
        {
            SelectColor();
        }
    }

    public void HoverToButton()
    {
        counter = 0.1f;
        isHovering = true;
    }
    private void SelectColor()
    {
        Material mat = Resources.Load($"Materials/rightBrush") as Material;
        switch (buttonColor)
        {
            case ButtonColor.Black:
                mat.color = Color.black;
                break;
            case ButtonColor.Red:
                mat.color = Color.red;
                break;
        }
    }
}
