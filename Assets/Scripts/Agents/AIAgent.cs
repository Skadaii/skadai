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
            if (!Target.gameObject.activeInHierarchy)
            {
                if(Target.TryGetComponent(out Agent agent))
                {
                    OnAgentExit(agent);
                }

                Target = null;

                return;
            }

            // Look at target position

            Vector3 desiredForward = Target.transform.position - transform.position;

            desiredForward = Vector3.Normalize(new Vector3(desiredForward.x, 0f, desiredForward.z));

            transform.forward = Vector3.Lerp(transform.forward, desiredForward, 0.1f);

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


    public override void ShootForward()
    {
        // instantiate bullet
        if (BulletPrefab && !GunCheckObstacle())
        {
            GameObject bullet = Instantiate(BulletPrefab, GunTransform.position, Quaternion.identity);

            if (bullet.TryGetComponent(out Bullet bulletComp))
            {
                bulletComp.IgnoreMask = ~(1 << gameObject.layer);
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (Target)
            {
                Vector3 bullerTrajectory = Target.transform.position - bullet.transform.position;

                bullerTrajectory = Vector3.Normalize(new Vector3(bullerTrajectory.x, 0f, bullerTrajectory.z));

                rb.AddForce(bullerTrajectory * BulletPower);
            }
            else
            {
                rb.AddForce(transform.forward * BulletPower);
            }
        }
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
