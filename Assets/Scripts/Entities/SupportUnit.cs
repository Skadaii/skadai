using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement))]
public class SupportUnit : Unit
{
    #region Variables

    private Unit  m_target;
    private Agent m_targetAgressor = null;

    [SerializeField] private float defendRange = 2f;


    #endregion

    #region Properties

    public Unit Target
    {
        get { return m_target; }

        set
        {
            //  Unregister the assigned support from the old target
            if (m_target != null) m_target.assignedSupport = null;

            //  If the new target does not have an assigned support
            if (value?.assignedSupport == null)
            {
                m_target = value;

                //  If the new target is valid register 'this' as the assigned support
                if (m_target)
                {
                    m_target.assignedSupport = this;
                    m_targetAgressor = m_target.agent.agressor;
                }
            }
        }
    }

    #endregion


    #region MonoBehaviour

    protected new void OnDisable()
    {
        base.OnDisable();

        Target = null;
    }

    private void OnDestroy()
    {
        Target = null;
    }

    #endregion


    #region Functions

    public float TargetCoverNeedFactor()
    {
        if (m_target != null && m_target.gameObject.activeInHierarchy)
        {

            bool targetIsBeingHurted = m_target.agent.agressor != null;
            bool isCurrentlyProtectingTarget = false;

            if(agent.agressor != null)
            {
                Vector3 agressorToAgent  = Vector3.Normalize(agent.agressor.transform.position - transform.position);
                Vector3 agressorToTarget = Vector3.Normalize(agent.agressor.transform.position - m_target.transform.position);

                isCurrentlyProtectingTarget = Vector3.Dot(agressorToTarget, agressorToAgent) >= 0.75f;
            }
            return Convert.ToSingle(targetIsBeingHurted || isCurrentlyProtectingTarget);
        }

        return 0f;
    }

    public void Cover()
    {
        movement.MoveTo(GetCoverPosition(m_target.transform.position, m_targetAgressor.transform.position));
    }

    public Vector3 GetCoverPosition(Vector3 targetPosition, Vector3 agressorPosition)
    {
        Vector3 dir = agressorPosition - targetPosition;

        return targetPosition + dir.normalized * defendRange;
    }

    public override bool HasDuty()
    {
        return TargetCoverNeedFactor() > 0f;
    }

    #endregion
}
