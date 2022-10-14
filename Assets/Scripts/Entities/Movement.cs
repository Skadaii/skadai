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

}
