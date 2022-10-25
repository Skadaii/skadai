using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class SupportUnit : Unit
{
    Unit target;

    [SerializeField] private float defendRange = 2f;

    private void Start()
    {
        //DevTest
        SetTarget(m_Squad.leader);
    }

    private void OnDestroy()
    {
        SetTarget(null);
    }

    public void SetTarget(Unit newTarget)
    {
        if (target != null && newTarget == null)
        {
            target.isBeingHealed = false;

            return;
        }

        if (newTarget == null || !newTarget.isBeingHealed)
        {
            target = newTarget;
        }
    }

    public float TargetCoverNeedFactor()
    {
        return target != null ? Convert.ToSingle(target.agent.Assaillant != null) : 0f;
    }

    public void Cover()
    {
        Vector3 dir = target.agent.Assaillant.transform.position - target.transform.position;
        movement.MoveTo(target.transform.position + dir.normalized * defendRange);
    }

    public override bool HasDuty()
    {
        return TargetCoverNeedFactor() > 0f;
    }
}
