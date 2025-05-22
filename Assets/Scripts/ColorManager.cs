using Unity.VisualScripting;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public enum ColorEnum
    {
        Red,
        Green,
        Blue,
        Gray,
        White,
        Black,
        Orange,
        Pink,
        Purple
    }

    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material grayMat;
    [SerializeField] private Material whiteMat;
    [SerializeField] private Material blackMat;
    [SerializeField] private Material orangeMat;
    [SerializeField] private Material pinkMat;
    [SerializeField] private Material purpleMat;

    [SerializeField] private Color selectedOutlineColor;
    public Color SelectedOutlineColor
    {
        get { return selectedOutlineColor; }
    }

    private Material selectedMaterial;

    public Material GetSelectedMaterial
    {
        get 
        {
            return selectedMaterial;
        }
    }

    [SerializeField] private ColorButton[] colorButtons;

    private void Start()
    {
        // initally it is blackk
        for(int i = 0; i < colorButtons.Length; i++)
        {
            if (colorButtons[i].buttonColor == ColorEnum.Black)
            {
                AssignColor(colorButtons[i]);
                break;
            }
        }

        transform.parent.gameObject.SetActive(false);
    }
    public void AssignColor(ColorButton _colorButton)
    {
        foreach(ColorButton _cButton in colorButtons)
        {
            _cButton.NotSelect();
        }
        switch (_colorButton.buttonColor)
        {
            case ColorEnum.Red:
                selectedMaterial = redMat;
                break;
            case ColorEnum.Green:
                selectedMaterial = greenMat;
                break;
            case ColorEnum.Blue:
                selectedMaterial = blueMat;
                break;
            case ColorEnum.Gray:
                selectedMaterial = grayMat;
                break;
            case ColorEnum.White:
                selectedMaterial = whiteMat;
                break;
            case ColorEnum.Black:
                selectedMaterial = blackMat;
                break;
            case ColorEnum.Orange:
                selectedMaterial = orangeMat;
                break;
            case ColorEnum.Pink:
                selectedMaterial = pinkMat;
                break;
            case ColorEnum.Purple:
                selectedMaterial = purpleMat;
                break;

        }

        _colorButton.SelectedAnimation();
    }


    public void HoverColorButton(ColorButton colorButton)
    {
        foreach(ColorButton color in colorButtons)
        {
            color.counter = 0;
        }

        colorButton.HoverAnimationStart();
    }

    


}
