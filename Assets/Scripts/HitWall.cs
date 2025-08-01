using UnityEngine;

public class HitWall : MonoBehaviour
{
    [SerializeField] private SurfaceInteraction.PDirection direction;
    public SurfaceInteraction.PDirection Direction
    {
        get { return direction; }
    }
}
