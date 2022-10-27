using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


[RequireComponent(typeof(SphereCollider))]
public class UnitAgent : AIAgent
{
    #region Variables

    NavMeshAgent NavMeshAgentInst;

    #endregion


    #region MonoBehaviour

    protected new void Awake()
    {
        base.Awake();

        NavMeshAgentInst = GetComponent<NavMeshAgent>();

        m_gunTransform = transform.Find("Body/Gun");

        if (m_gunTransform == null)
            Debug.LogWarning("could not find gun transform");
    }

    private new void Update()
    {
        base.Update();

        if(Target == null) CheckTarget();
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        OnHit.AddListener(SetTargetToAgressor);
    }

    protected new void OnDisable()
    {
        base.OnDisable();
        OnHit.RemoveListener(SetTargetToAgressor);
    }

    #endregion

    #region Functions

    public void StopMove() => NavMeshAgentInst.isStopped = true;
    
    public void MoveTo(Vector3 dest)
    {
        NavMeshAgentInst.isStopped = false;
        NavMeshAgentInst.SetDestination(dest);
    }


    public override void ShootAtTarget()
    {
        base.ShootAtTarget();
    }

    private void SetTargetToAgressor()
    {
        if (AgentTrespassers.Contains(agressor))
        {
            Target ??= agressor.gameObject;
        }
    }
    //  Consideration functions

    public override float HasTarget()
    {
        return base.HasTarget();
    }

    #endregion
}
