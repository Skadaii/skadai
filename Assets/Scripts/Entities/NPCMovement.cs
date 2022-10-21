using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : Movement
{
    private NavMeshAgent m_NavAgent = null;

    void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (TransformTarget)
            MoveTo(TransformTarget.position);
    }

    public override void MoveTo(Vector3 target)
    {
        base.MoveTo(target);
        m_NavAgent.SetDestination(target);
    }

    public override void MoveTo(Transform target)
    {
        base.MoveTo(target);
        m_NavAgent.SetDestination(target.position);
    }

    public override void MoveToward(Vector3 velocity)
    {
        TransformTarget = null;
        PositionTarget = null;

        m_NavAgent.Move(velocity);
    }


    public override bool HasReachedPos(float epsilon)
    {
        return m_NavAgent.remainingDistance - m_NavAgent.stoppingDistance <= epsilon;
    }
}
