using UnityEngine;

public class ImageRandomizer : MonoBehaviour
{
    [SerializeField] private Sprite cat;
    [SerializeField] private Sprite dog;
    [SerializeField] private UnityEngine.UI.Image image;

    private void Start()
    {
        int num = Random.Range(0, 2);
        if(num == 0)
        {
            image.sprite = cat;
        }
        else
        {
            image.sprite = dog;
        }
    }
}
