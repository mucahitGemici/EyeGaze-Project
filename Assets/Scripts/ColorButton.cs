using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ColorButton : MonoBehaviour
{
    public Image outlineImage;
    [SerializeField] private ColorManager colorManager;

    // this is gonna be activated with raycasting (left controller raycast)
    [HideInInspector] public bool isHovering;
    [HideInInspector] public bool wantsHovering;


    private bool isSelected;
    

    [SerializeField] public ColorManager.ColorEnum buttonColor;

    [HideInInspector] public float counter;

    private float normalScale;
    private float hoverScale;
    private float pressedScale;

    private void Awake()
    {
        normalScale = transform.localScale.x;
        hoverScale = normalScale + 0.2f;
        pressedScale = normalScale - 0.2f;
    }

    private void Update()
    {
        if(counter > 0) counter -= Time.deltaTime;
        if(counter <= 0 && isHovering == true)
        {
            HoverAnimationEnd();
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
        if(isSelected == false)
        {
        colorManager.HoverColorButton(this);
        }
    }
    private void SelectColor()
    {
        colorManager.AssignColor(this);
    }

    public void HoverAnimationStart()
    {
        if(isSelected == false)
        {
            transform.DOScale(hoverScale, 0.5f);
            isHovering = true;
            counter = 0.25f;
        }
    }

    private void HoverAnimationEnd()
    {
        if(isSelected == false)
        {
            transform.DOScale(normalScale, 0.5f);
        }
    }

    public void SelectedAnimation()
    {
        if(isSelected == false)
        {
            transform.DOScale(pressedScale, 0.5f);
            isSelected = true;
            outlineImage.color = colorManager.SelectedOutlineColor;
        }
    }

    public void NotSelect()
    {
        if(isSelected == true)
        {
            isSelected = false;
            transform.DOScale(normalScale, 0.5f);
            outlineImage.color = Color.white;
        }
    }
}
