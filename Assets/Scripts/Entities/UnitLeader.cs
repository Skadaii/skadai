using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLeader : Unit
{
    protected new void OnEnable()
    {
        base.OnEnable();

        agent?.OnHit.AddListener(CallForCover);
    }

    protected new void OnDisable()
    {
        base.OnDisable();

        agent?.OnHit.RemoveListener(CallForCover);
    }

    public override void SetSquad(UnitSquad squad)
    {
        // If the leader is virtual
        if (!agent)
        {
            base.SetSquad(squad);
            return;
        }
        base.SetSquad(squad);
    }

    public void CallForCover()
    {
        m_Squad.AssignSupportTo(this);
    }
}
