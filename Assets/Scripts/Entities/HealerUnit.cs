using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class HealerUnit : Unit
{
    [SerializeField] private int   m_healPower = 5;
    [SerializeField] private float m_healCooldown = 1f;
    [SerializeField] private float m_healingRadius = 2f;

    private bool  m_canHeal = false;
    private float m_lastTimeHealing = 0f;

    public Unit target { get; private set; }

    private GameObject m_healFX;
    private ParticleSystem m_healFParticles;

    protected new void Awake()
    {
        base.Awake();

        m_healFX = Resources.Load("FXs/ParticleHealing") as GameObject;
        if (m_healFX)
        {
            GameObject instance = Instantiate(m_healFX, null);
            m_healFParticles = instance.GetComponentInChildren<ParticleSystem>();
            m_healFParticles?.Stop(true);
        }
    }

    private void Update()
    {
        if(!m_canHeal)
        {
            if(Time.time - m_lastTimeHealing >= m_healCooldown)
            {
                m_lastTimeHealing = Time.time;
                m_canHeal = true;
            }
        }
    }

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

        TryStopParticles();
    }

    public float TargetHealNeedFactor()
    {
        return target != null && target.gameObject.activeInHierarchy ? Mathf.Max(1f - target.agent.GetLifePercent(), 0f) : 0f;
    }

    public void Heal()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 position = targetPos + Vector3.Normalize(transform.position - targetPos) * m_healingRadius;
        movement.MoveTo(position);

        if (Vector3.SqrMagnitude(position - transform.position) <= m_healingRadius * m_healingRadius)
        {
            TryStartParticles();

            if (m_canHeal && target.agent.AddHealth(m_healPower))
            {
                // Stop healing

                SetTarget(null); 
            }

            m_canHeal = false;
        }
        else
        {
            TryStopParticles();
        }
    }

    private void TryStartParticles()
    {
        if (m_healFParticles && !m_healFParticles.isPlaying)
        {
            
            m_healFParticles.Play();
            m_healFParticles.transform.SetParent(target.transform);
            m_healFParticles.transform.localPosition = Vector3.zero;
        }
    }

    private void TryStopParticles()
    {
        if (m_healFParticles && m_healFParticles.isPlaying)
        {
            m_healFParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            m_healFParticles.transform.SetParent(null);
        }
    }

    public override bool HasDuty()
    {
        return TargetHealNeedFactor() > 0f;
    }
}
