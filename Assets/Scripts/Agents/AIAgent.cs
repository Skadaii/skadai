using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class AIAgent : Agent, IDamageable
{
    #region Variables

    protected SphereCollider m_trigger;

    protected HashSet<Agent> m_agentTrespassers = new HashSet<Agent>();

    protected GameObject m_target = null;

    [SerializeField] private float m_rotationSpeed = 10f;

    #endregion

    #region MonoBehaviour

    protected new void Awake()
    {
        base.Awake();

        m_currentHealth = m_maxHealth;

        m_trigger = GetComponent<SphereCollider>();

        m_trigger.enabled = false;
    }

    protected new void Start()
    {
        base.Start();

        m_trigger.enabled = true;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger &&other.gameObject.TryGetComponent(out Agent agent) && agent.AgentTeam != m_agentTeam)
        {
            OnAgentEnter(agent);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.gameObject.TryGetComponent(out Agent agent) && agent.AgentTeam != m_agentTeam)
        {
            OnAgentExit(agent);
        }
    }

    protected new void Update()
    {
        base.Update();

        if (m_target && !m_target.gameObject.activeInHierarchy)
        {
            if(m_target.TryGetComponent(out Agent agent))
            {
                OnAgentExit(agent);
            }

            m_target = null;
        }
    }


    #endregion

    #region Functions

    public virtual void ShootAtTarget()
    {
        if (m_target != null)
        {
            ShootAt(m_target);
        }
        else if (agressor != null && agressor.gameObject.activeInHierarchy)
        {
            ShootAt(agressor.gameObject);
        }
    }

    public void ShootAt(GameObject target)
    {
        if (!target) return;

        // Look at target position
        
        Vector3 desiredForward = target.transform.position - transform.position;

        desiredForward = Vector3.Normalize(new Vector3(desiredForward.x, 0f, desiredForward.z));

        transform.forward = Vector3.Slerp(transform.forward, desiredForward, Time.deltaTime * m_rotationSpeed);

        //  Can shoot ?
        if (Vector3.SqrMagnitude(transform.forward - desiredForward) < 0.1f && Time.time >= m_nextShootDate)
        {
            m_nextShootDate = Time.time + m_shootFrequency;
            ShootForward();
        }
    }

    protected override void OnHealthChange()
    {
        base.OnHealthChange();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        gameObject.SetActive(false);
    }

    public void SetTarget(GameObject obj)
    {
        m_target = obj;
    }

    public virtual void OnAgentEnter(Agent agent)
    {
        m_agentTrespassers.Add(agent);
    }

    public virtual void OnAgentExit(Agent agent)
    {
        if (m_target == agent.gameObject) m_target = null;

        m_agentTrespassers.Remove(agent);
    }

    protected override Vector3 GetBulletTrajectory()
    {
        return m_target == null ? 
            transform.forward : 
            Vector3.Normalize(new Vector3(m_target.transform.position.x - m_gunTransform.position.x, 0f, m_target.transform.position.z - m_gunTransform.position.z));
    }


    protected void CheckTarget()
    {
        float minDistance = float.MaxValue;

        Agent newTarget = null;

        foreach(Agent agent in m_agentTrespassers)
        { 
            float distance = Vector3.SqrMagnitude(transform.position - agent.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                newTarget = agent;
            }
        }

        SetTarget(newTarget != null ? newTarget.gameObject : null);
    }


    //  Consideration functions

    public virtual float HasTarget()
    {
        return System.Convert.ToSingle(m_target != null || (agressor != null && agressor.gameObject.activeInHierarchy));
    }

    #endregion
}
