using System;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [Serializable]
    public enum Condition
    {
        DirectSketching,
        IndirectSketching
    }

    [SerializeField] private Condition condition;
    public Condition GetExperimentCondition
    {
        get { return condition; }
    }
}
