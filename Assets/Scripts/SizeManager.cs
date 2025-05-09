using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SizeManager : MonoBehaviour
{
    [SerializeField] private Slider sizeSlider;
    Vector2 joystick;
    private void Update()
    {
        joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        
    }

    private void FixedUpdate()
    {
        if (joystick.x > 0)
        {
            // increase size
            sizeSlider.value += 0.05f;
        }
        else if (joystick.x < 0)
        {
            // reduce size
            sizeSlider.value -= 0.05f;
        }
    }
}
