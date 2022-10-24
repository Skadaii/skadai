using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLeader : Unit
{
    void OnDestroy()
    {
        if (m_Squad && agent) // Only if the leader is virtual
            agent.OnHit.RemoveListener(m_Squad.AskDefenderUnit);
    }

    public override void SetSquad(UnitSquad squad)
    {
        // If the leader is virtual
        if (!agent)
        {
            base.SetSquad(squad);
            return;
        }

        if (m_Squad)
            agent.OnHit.RemoveListener(m_Squad.AskDefenderUnit);

        base.SetSquad(squad);

        if (m_Squad)
            agent.OnHit.AddListener(m_Squad.AskDefenderUnit);
    }
}
