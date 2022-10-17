using UnityEngine;

public class FormationRule : ScriptableObject
{
    virtual public Vector3 ComputePosition(Transform center, int index)
    {
        return center.position;
    }
}
