using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class HealerUnit : Unit
{
    [SerializeField] private int   healPower = 5;
    [SerializeField] private float healingRadius = 1.25f;

    public Unit target { get; private set; }

    private void OnDestroy()
    {
        SetTarget(null);
    }

    public void SetTarget(Unit newTarget)
    {
        //  Unregister the assigned healer from the old target
        if(target != null) target.assignedHealer = null;

        //  If the new target already has an assigned healer
        if (newTarget?.assignedHealer != null) return;

        target = newTarget;

        //  If the new target is valid register 'this' as the assigned healer
        if (target) target.assignedHealer = this;
    }

    public float TargetHealNeedFactor()
    {
        return target != null ? Mathf.Max(1f - target.agent.GetLifePercent(), 0f) : 0f;
    }

    public void Heal()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 position = targetPos + Vector3.Normalize(transform.position - targetPos) * healingRadius;
        movement.MoveTo(position);

        if (Vector3.SqrMagnitude(position - transform.position) <= healingRadius * healingRadius)
        {
            if(target.agent.AddHealth(healPower))
            {
                SetTarget(null);
            }
        }
    }

    public override bool HasDuty()
    {
        return TargetHealNeedFactor() > 0f;
    }
}
