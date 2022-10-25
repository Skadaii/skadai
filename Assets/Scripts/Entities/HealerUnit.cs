using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class HealerUnit : Unit
{
    [SerializeField] private int   healPower = 5;
    [SerializeField] private float healingRadius = 1.25f;

    Unit target;

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

    public float LeaderHealNeedFactor()
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
            target.isBeingHealed = !target.agent.AddHealth(healPower);
        }
    }

    public override bool HasDuty()
    {
        return LeaderHealNeedFactor() > 0f;
    }
}
