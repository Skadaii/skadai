using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;

public class UnitSquad : MonoBehaviour
{
    public FormationRule formation = null;

    [SerializeField] private GameObject virtualLeaderPrefab = null;

    public UnitLeader leader = null;

    private List<Unit> units = new List<Unit>();
    private Vector3[] unitPositions;

    [SerializeField] private float defendRange = 2f;
    private Unit defender = null;
    private GameObject leaderAttacker = null;

    public List<Unit> Units
    {
        get => units;
        set
        {  
            units = value;

            foreach (Unit unit in units)
                unit.SetSquad(this);

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
        leader ??= CreateVirtuaLeader();

        leader.SetSquad(this);

        leader.GetComponent<Movement>().OnMoveChange.AddListener(UpdatePositions);
    }

    private void OnDestroy()
    {
        if (leader && leader.TryGetComponent(out Movement leaderMovement))
            leaderMovement.OnMoveChange.RemoveListener(UpdatePositions);
    }

    public void AskDefenderUnit(GameObject attacker)
    {
        leaderAttacker = attacker;

        Unit nearestUnit = null;
        float minDist = Mathf.Infinity;

        // Get the nearest unit to the line leader/attacker 
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];

            float dist = HandleUtility.DistancePointLine(unit.transform.position, leader.transform.position, leaderAttacker.transform.position);
            if (dist < minDist)
            {
                nearestUnit = unit;
                minDist = dist;
            }
        }

        defender = nearestUnit;
    }

    public void UpdatePositions()
    {
        if (defender)
        {
            Vector3 direction = Vector3.Normalize(leaderAttacker.transform.position - leader.transform.position);

            Vector3 pos = leader.transform.position + direction * defendRange;

            defender.movement.MoveTo(pos);
        }

        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];

            if (unit == defender)
                continue;

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

    UnitLeader CreateVirtuaLeader()
    {
        GameObject leader = Instantiate(virtualLeaderPrefab);

        leader.GetComponent<PatrolAction>().patrolPoints = GetComponentsInChildren<Transform>();

        return leader.GetComponent<UnitLeader>();
    }
}
