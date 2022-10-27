using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class SupportUnit : Unit
{
    public Unit target { get; private set; }

    public float defendRange = 2f;
    private Agent targetAgressor;


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
            targetAgressor = target.agent.agressor;
        }
    }

    public float TargetCoverNeedFactor()
    {
        if (target != null && target.gameObject.activeInHierarchy)
        {

            bool targetIsBeingHurted = target.agent.agressor != null;
            bool isCurrentlyProtectingTarget = false;

            if(agent.agressor != null)
            {
                Vector3 agressorToAgent  = Vector3.Normalize(agent.agressor.transform.position - transform.position);
                Vector3 agressorToTarget = Vector3.Normalize(agent.agressor.transform.position - target.transform.position);

                isCurrentlyProtectingTarget = Vector3.Dot(agressorToTarget, agressorToAgent) >= 0.8f;
            }
            return Convert.ToSingle(targetIsBeingHurted || isCurrentlyProtectingTarget);
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
