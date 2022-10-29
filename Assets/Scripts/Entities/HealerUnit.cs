using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class HealerUnit : Unit
{
    #region Variables

    [SerializeField] private int   m_healPower = 5;
    [SerializeField] private float m_healCooldown = 1f;
    [SerializeField] private float m_healingRadius = 2f;

    private bool  m_canHeal = false;
    private float m_lastTimeHealing = 0f;

    [SerializeField] private Unit m_target;

    private GameObject     m_healFX;
    private ParticleSystem m_healFParticles;

    #endregion


    #region Properties

    public Unit Target
    {
        get { return m_target;  }

        set
        {
            //  Unregister the assigned healer from the old target
            if (m_target != null) m_target.assignedHealer = null;

            //  If the new target does not have an assigned healer
            if (value == null || value.assignedHealer == null)
            {
                m_target = value;

                //  If the new target is valid register 'this' as the assigned healer
                if (m_target) m_target.assignedHealer = this;
            }
        }
    }

    #endregion


    #region MonoBehaviour

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

    protected void Update()
    {
        //  Healer timer
        if(!m_canHeal)
        {
            if(Time.time - m_lastTimeHealing >= m_healCooldown)
            {
                m_lastTimeHealing = Time.time;
                m_canHeal = true;
            }
        }
    }

    protected new void OnEnable()
    {
        base.OnEnable();

        agent?.OnHit.AddListener(CallForCover);
    }

    protected new void OnDisable()
    {
        base.OnDisable();

        Target = null;

        agent?.OnHit.RemoveListener(CallForCover);
    }

    private void OnDestroy()
    {
        Target = null;
    }

    #endregion


    #region Functions

    public float TargetHealNeedFactor()
    {
        return m_target != null && m_target.gameObject.activeInHierarchy ? Mathf.Max(1f - m_target.agent.GetLifePercent(), 0f) : 0f;
    }

    public void Heal()
    {
        Vector3 targetPos = m_target.transform.position;
        Vector3 position  = targetPos + Vector3.Normalize(transform.position - targetPos) * m_healingRadius;
        movement.MoveTo(position);

        if (Vector3.SqrMagnitude(position - transform.position) <= m_healingRadius * m_healingRadius)
        {
            TryStartParticles();

            if (m_canHeal && m_target.agent.AddHealth(m_healPower))
            {
                // Stop healing

                Target = null; 
            }

            m_canHeal = false;
        }
    }

    private void TryStartParticles()
    {
        if (m_healFParticles && !m_healFParticles.isPlaying)
        {
            m_healFParticles.Play();
            m_healFParticles.transform.SetParent(m_target.transform);
            m_healFParticles.transform.localPosition = Vector3.zero;
        }
    }

    public void CallForCover()
    {
        m_Squad.AssignSupportTo(this);
    }

    public override bool HasDuty()
    {
        return TargetHealNeedFactor() > 0f;
    }

    #endregion
}
