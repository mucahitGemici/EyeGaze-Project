using UnityEngine;
using UnityEngine.UI;
public class ColorPaletteActivation : MonoBehaviour
{
    [SerializeField] private Image activatingImage;
    [SerializeField] private Image deactivatingImage;
    [SerializeField] private Transform colorPaletteReference;

    private bool currentState;

    private float resetCounter;
    private float counter;
    public float Counter
    {
        set {  counter = value; }
    }

    public bool ColorPaletteActivated
    {
        get { return colorPaletteReference.gameObject.activeSelf; }
    }

    private void Update()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 3.5f;
        transform.position = targetPosition;
        

        if(counter > 0f && counter < 1f)
        {
            currentState = colorPaletteReference.gameObject.activeSelf;
            if(currentState == false)
            {
                // show activation image
                activatingImage.gameObject.SetActive(true);
                activatingImage.fillAmount = counter;
            }
            else
            {
                // show deactivation image
                deactivatingImage.gameObject.SetActive(true);
                deactivatingImage.fillAmount = counter;
            }
        }
        else if(counter >= 1f)
        {
            // change state
            colorPaletteReference.gameObject.SetActive(!currentState);
            activatingImage.fillAmount = 0;
            activatingImage.gameObject.SetActive(false);
            deactivatingImage.fillAmount = 0;
            deactivatingImage.gameObject.SetActive(false);
        }
        

        if(resetCounter > 0f)
        {
            resetCounter -= Time.deltaTime;
        }
    }

    public void ResetState()
    {
        // do not change state but reset the activating and deactivating images
        // because user stopped pressing B before counter reached to 1
        activatingImage.fillAmount = 0;
        activatingImage.gameObject.SetActive(false);
        deactivatingImage.fillAmount = 0;
        deactivatingImage.gameObject.SetActive(false);
    }
}
