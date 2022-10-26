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

    public float defendRange = 2f;
    //private Unit defender = null;
    //private Unit healer = null;
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

    public void AssignHealerTo(Unit target)
    {
        Vector3 targetPosition = target.transform.position;
        float lowestSqrDistance = float.MaxValue;
        HealerUnit nearestHealer = null;

        foreach (Unit unit in units)
        {
            if(unit == target)
                continue;

            HealerUnit healer = unit as HealerUnit;

            if (healer == null)
                continue;

            if (healer.target == leader)
                continue;

            if(target != leader)
            {
                if (healer.target != null) 
                    continue;

                if (healer.agent.CurrentHP <= target.agent.CurrentHP) continue;
            }

            Vector3 healerPosition = healer.transform.position;
            float sqrDistance = Vector3.SqrMagnitude(healerPosition - targetPosition);

            if(lowestSqrDistance > sqrDistance)
            {
                lowestSqrDistance = sqrDistance;
                nearestHealer = healer;
            }
        }

        nearestHealer?.SetTarget(target);
    }

    public void AssignSupportTo(Unit target)
    {
        Vector3 targetPosition = target.transform.position;
        float lowestSqrDistance = float.MaxValue;
        SupportUnit nearestSupport = null;

        if (target.agent.Aggressor == null) return;

        foreach (Unit unit in units)
        {
            if (unit == target)
                continue;

            SupportUnit support = unit as SupportUnit;

            if (support == null)
                continue;

            Vector3 dir = target.agent.Aggressor.transform.position - targetPosition;
            float sqrDistance = Vector3.SqrMagnitude(support.transform.position - (targetPosition + dir.normalized * support.defendRange));

            if (lowestSqrDistance > sqrDistance)
            {
                lowestSqrDistance = sqrDistance;
                nearestSupport = support;
            }
        }

        nearestSupport?.SetTarget(target);
    }

    public void UpdatePositions()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];

            if (!unit.gameObject.activeInHierarchy || unit.HasDuty() /*|| unit == defender || unit == healer*/) continue;

            Vector3 pos = ComputeUnitPosition(i);
            units[i].movement.MoveTo(pos);
            unitPositions[i] = pos;
        }
    }

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
        GameObject leader = Instantiate(virtualLeaderPrefab, transform.position, transform.rotation);

        leader.GetComponent<PatrolAction>().patrolPoints = GetComponentsInChildren<Transform>();

        return leader.GetComponent<UnitLeader>();
    }
}
