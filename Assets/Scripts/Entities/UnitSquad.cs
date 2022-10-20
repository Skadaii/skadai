using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSquad : MonoBehaviour
{
    public FormationRule formation = null;

    public UnitLeader leader = null;

    private List<Unit> units = new List<Unit>();
    private Vector3[] unitPositions;

    public List<Unit> Units
    {
        get => units;
        set
        {  
            units = value;

            foreach (Unit unit in units)
                unit.m_Squad = this;

            unitPositions = new Vector3[units.Count];
        }
    }

    private void Start()
    {
        // If leader is null, set a virtual one
        leader ??= CreateVirtuaLeader("Leader");

        leader.m_Squad = this;
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = ComputeUnitPosition(i);
            units[i].movement.MoveTo(pos);
            unitPositions[i] = pos;
        }
    }
    public void ShootToPosition(Vector3 position)
    {
        for (int i = 0; i < units.Count; i++)
            units[i].agent.ShootToPosition(position);
    }

    Vector3 ComputeUnitPosition(int index)
    {
        if (formation)
        {
            return formation.ComputePosition(leader.transform, index);
        }
        return leader.transform.position;
    }

    UnitLeader CreateVirtuaLeader(string leaderName)
    {
        GameObject leaderGO = new GameObject(leaderName);
        leaderGO.AddComponent<Movement>();

        return leaderGO.AddComponent<UnitLeader>();
    }
}
