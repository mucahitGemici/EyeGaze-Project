using Oculus.Interaction.Locomotion;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Transform target;
    public PlayerLocomotor locomotor;

    public void Spawn()
    {
        locomotor.MovePlayer(target.position, LocomotionEvent.TranslationType.Absolute);
        locomotor.RotatePlayer(target.rotation, LocomotionEvent.RotationType.Absolute);
    }

}
