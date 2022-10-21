using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class AIAgent : Agent, IDamageable
{
    protected SphereCollider Trigger;

    protected List<Agent> AgentTrespassers = new List<Agent>();

    protected GameObject Target = null;

    #region MonoBehaviour

    protected new void Awake()
    {
        base.Awake();

        CurrentHP = MaxHP;

        Trigger = GetComponent<SphereCollider>();
    }

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.isTrigger && other.gameObject.TryGetComponent(out Agent agent))
        {
            OnAgentEnter(agent);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.gameObject.TryGetComponent(out Agent agent))
        {
            OnAgentExit(agent);
        }
    }

    protected void Update()
    {
        if (Target)
        {
            // Look at target position

            Vector3 desiredForward = Target.transform.position - transform.position;
            desiredForward = Vector3.Normalize(new Vector3(desiredForward.x, 0f, desiredForward.z));

            transform.forward = Vector3.SmoothDamp(transform.forward, desiredForward, ref DeltaVel, 0.05f);

            //  Can shoot ?
            if (Vector3.SqrMagnitude(transform.forward - desiredForward) < 0.1f && Time.time >= NextShootDate)
            {
                NextShootDate = Time.time + ShootFrequency;
                ShootForward();
            }
        }
    }

    private void OnDrawGizmos()
    {
    }

    #endregion

    #region ActionMethods

    protected override void OnHealthChange()
    {
        base.OnHealthChange();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        gameObject.SetActive(false);
    }

    #endregion

    #region UtilsMethod

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
        AgentTrespassers.Remove(agent);
    }

    #endregion
}
