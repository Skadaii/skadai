using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class AIAgent : Agent, IDamageable
{
    #region Variables

    protected SphereCollider Trigger;

    protected List<Agent> AgentTrespassers = new List<Agent>();

    protected GameObject Target = null;

    [SerializeField] private float rotationSpeed = 10f;

    #endregion

    #region MonoBehaviour

    protected new void Awake()
    {
        base.Awake();

        m_currentHealth = m_maxHealth;

        Trigger = GetComponent<SphereCollider>();

        Trigger.enabled = false;
    }

    protected void Start()
    {
        Trigger.enabled = true;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger &&other.gameObject.TryGetComponent(out Agent agent) &&agent.team != team)
        {
            Debug.Log(agent.GetType());
            OnAgentEnter(agent);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.gameObject.TryGetComponent(out Agent agent) && agent.team != team)
        {
            OnAgentExit(agent);
        }
    }

    protected void Update()
    {
        if (Target && !Target.gameObject.activeInHierarchy)
        {
            if(Target.TryGetComponent(out Agent agent))
            {
                OnAgentExit(agent);
            }

            Target = null;
        }
    }


    #endregion

    #region Functions

    public virtual void ShootAtTarget()
    {
        if (!Target) return;

        // Look at target position
        
        Vector3 desiredForward = Target.transform.position - transform.position;

        desiredForward = Vector3.Normalize(new Vector3(desiredForward.x, 0f, desiredForward.z));

        transform.forward = Vector3.Slerp(transform.forward, desiredForward, Time.deltaTime * rotationSpeed);

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
        Target = obj;
    }

    public virtual void OnAgentEnter(Agent agent)
    {
        AgentTrespassers.Add(agent);
    }

    public virtual void OnAgentExit(Agent agent)
    {
        if (Target == agent.gameObject) Target = null;

        AgentTrespassers.Remove(agent);
    }

    protected override Vector3 GetBulletTrajectory()
    {
        return Target == null ? 
            transform.forward : 
            Vector3.Normalize(new Vector3(Target.transform.position.x - m_gunTransform.position.x, 0f, Target.transform.position.z - m_gunTransform.position.z));
    }


    protected void CheckTarget()
    {
        float minDistance = float.MaxValue;

        Agent newTarget = null;

        AgentTrespassers.ForEach(agent =>
        {
            float distance = Vector3.SqrMagnitude(transform.position - agent.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                newTarget = agent;
            }
        });

        SetTarget(newTarget != null ? newTarget.gameObject : null);
    }


    //  Consideration functions

    public virtual float HasTarget()
    {
        return System.Convert.ToSingle(Target != null);
    }

    #endregion
}
