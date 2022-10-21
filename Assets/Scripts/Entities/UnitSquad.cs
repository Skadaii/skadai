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
        OnInitializeLeader();
    }

    private void OnInitializeLeader()
    {
        // If leader is null, set a virtual one
        leader ??= CreateVirtuaLeader("Leader");

        leader.m_Squad = this;

        leader.GetComponent<Movement>().OnMoveChange.AddListener(UpdatePosition);
    }

    private void OnDestroy()
    {
        leader?.GetComponent<Movement>().OnMoveChange.RemoveListener(UpdatePosition);
    }

    private void Update()
    {
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

    /*public void ShootToPosition(Vector3 position)
    {
        for (int i = 0; i < units.Count; i++)
            units[i].agent.ShootToPosition(position);
    }*/

    public void SetTarget(GameObject target)
    {
        for (int i = 0; i < units.Count; i++)
        {
            AIAgent ai = units[i].agent as AIAgent;
            if (ai != null)
            {
                ai.SetTarget(target);
            }
        }
    }

    Vector3 ComputeUnitPosition(int index)
    {
        if (formation)
        {
            if (leader.movement.PositionTarget is not null)
                return formation.ComputePosition(leader.movement.PositionTarget.Value, leader.transform.rotation, index);

            if (leader.movement.TransformTarget is not null)
            {
                Transform target = leader.movement.TransformTarget;
                return formation.ComputePosition(target.position, target.rotation, index);
            }

            return formation.ComputePosition(leader.transform.position, leader.transform.rotation, index);
        }
        return leader.transform.position;
    }

    UnitLeader CreateVirtuaLeader(string leaderName)
    {
        GameObject leaderGO = new GameObject(leaderName);
        leaderGO.AddComponent<NPCMovement>();
        leaderGO.AddComponent<PatrolAction>().patrolPoints = GetComponentsInChildren<Transform>();

        return leaderGO.AddComponent<UnitLeader>();
    }
}
