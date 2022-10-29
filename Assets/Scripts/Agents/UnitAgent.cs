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

        m_explosionFX = Resources.Load("FXs/ParticleBloodExplode") as GameObject;
        HurtFX        = Resources.Load("FXs/ParticleBlood") as GameObject;

        NavMeshAgentInst = GetComponent<NavMeshAgent>();
    }

    private new void Update()
    {
        base.Update();

        if (m_target == null)
        {
            CheckTarget();
        }
    }

    protected new void OnEnable()
    {
        base.OnEnable();
    }

    protected new void OnDisable()
    {
        base.OnDisable();
    }

    #endregion


    #region Functions

    public void StopMove() => NavMeshAgentInst.isStopped = true;
    
    public void MoveTo(Vector3 dest)
    {
        NavMeshAgentInst.isStopped = false;
        NavMeshAgentInst.SetDestination(dest);
    }

    public override float HasTarget() => base.HasTarget();
    public override void ShootAtTarget() => base.ShootAtTarget();

    #endregion
}
