using Oculus.Interaction.Input;
using UnityEngine;

public class EyeFiltering : MonoBehaviour
{
    [SerializeField] private Transform eyeReference;

    OneEuroFilter<Quaternion> quaternionFilter;

    private float filterFrequency = 100f;
    private void Start()
    {
        quaternionFilter = new OneEuroFilter<Quaternion>(filterFrequency);
    }
    private void Update()
    {
        transform.position = eyeReference.position;
        transform.rotation = quaternionFilter.Filter(eyeReference.rotation);
    }
}
