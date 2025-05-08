using UnityEngine;

public class GazeReader : MonoBehaviour
{
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private LayerMask eyesHitLayers;

    [SerializeField] private LineRenderer lineRenderer;

    private void Update()
    {
        Vector3 eyePositionAverage = (leftEye.position + rightEye.position) / 2f;
        Vector3 eyesForwardDirectionAverage = ((leftEye.forward + rightEye.forward) / 2f).normalized;

        RaycastHit hit;
        if(Physics.Raycast(eyePositionAverage, eyesForwardDirectionAverage, out hit, Mathf.Infinity, eyesHitLayers))
        {
            GameObject hitGameObject = hit.collider.gameObject;
            int objectLayerNumber = hitGameObject.layer;

            lineRenderer.SetPosition(0, eyePositionAverage);
            lineRenderer.SetPosition(1, hit.point);

            if(objectLayerNumber == 6)
            {
                hitGameObject.GetComponent<GazeFocus>().FocusWithGaze();
            }
        }
    }
}
