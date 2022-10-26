using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class SupportUnit : Unit
{
    public Unit target { get; private set; }

    public float defendRange = 2f;
    private GameObject targetAgressor;


    private void OnDestroy()
    {
        SetTarget(null);
    }

    public void SetTarget(Unit newTarget)
    {
        //  Unregister the assigned healer from the old target
        if (target != null) target.assignedSupport = null;

        //  If the new target already has an assigned healer
        if (newTarget?.assignedSupport != null) return;

        target = newTarget;

        //  If the new target is valid register 'this' as the assigned healer
        if (target)
        {
            target.assignedSupport = this;
            targetAgressor = target.agent.Aggressor;
        }
    }

    public float TargetCoverNeedFactor()
    {
        if (target != null)
        {
            return Convert.ToSingle(target.agent.Aggressor != null || agent.Aggressor != null);
        }

        return 0f;
    }

    public void Cover()
    {
        Vector3 dir = targetAgressor.transform.position - target.transform.position;
        movement.MoveTo(target.transform.position + dir.normalized * defendRange);
    }

    public override bool HasDuty()
    {
        return TargetCoverNeedFactor() > 0f;
    }
}
