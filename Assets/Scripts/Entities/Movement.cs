using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Transform transformTarget = null;
    private Vector3? positionTarget = null;

    public Transform TransformTarget
    {
        get => transformTarget;
        set
        {
            transformTarget = value;
            positionTarget = null;
        } 
    }

    public Vector3? PositionTarget
    {
        get => positionTarget;
        set
        {
            positionTarget = value;
            transformTarget = null;
        }
    }

    public virtual void MoveToward(Vector3 velocity) { }
    public virtual void MoveTo(Vector3 target) => PositionTarget = target;
    public virtual void MoveTo(Transform target) => TransformTarget = target;

    public virtual bool HasReachedPos(float epsilon)
    {
        if (positionTarget is null && TransformTarget is null)
            return true;

        Vector3 difference = (positionTarget ?? TransformTarget.position) - transform.position;

        return Vector3.SqrMagnitude(difference) <= epsilon;
    }
}
