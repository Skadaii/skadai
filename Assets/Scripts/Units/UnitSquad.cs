using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;

public class UnitSquad : MonoBehaviour
{
    #region Variables

    public FormationRule formation = null;

    [SerializeField] private GameObject virtualLeaderPrefab = null;
    [SerializeField] private ScriptableTeam squadTeam;

    public ScriptableTeam SquadTeam {get {return squadTeam;}}

    public UnitLeader leader = null;

    private List<Unit> units = new List<Unit>();
    private Vector3[] unitPositions;

    public float defendRange = 2f;
    public float alertMaxDistance = 10f;

    #endregion


    #region Properties

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

    #endregion


    #region MonoBehaviour

    private void Start()
    {
        OnInitializeLeader();
    }

    private void OnDestroy()
    {
        if (leader && leader.TryGetComponent(out Movement leaderMovement))
            leaderMovement.OnMoveChange.RemoveListener(UpdatePositions);
    }

    #endregion


    #region Functions

    private void OnInitializeLeader()
    {
        // If leader is null, set a virtual one
        leader ??= CreateVirtuaLeader();

        leader.SetSquad(this);
        
        if(leader.agent) leader.agent.AgentTeam = squadTeam;

        leader.GetComponent<Movement>().OnMoveChange.AddListener(UpdatePositions);
    }

    public bool AssignHealerTo(Unit target)
    {
        float lowestSqrDistance = float.MaxValue;
        HealerUnit nearestHealer = null;

        foreach (Unit unit in units)
        {
            if(unit == target) continue;

            HealerUnit healer = unit as HealerUnit;

            if (healer == null || healer.Target == leader) continue;

            //  If the target is not the leader (who is priorized)
            if(target != leader)
            {
                //  If the current healer already has a target, skip.
                if (healer.Target != null && healer.TargetHealNeedFactor() > 0f) continue;
            }

            Vector3 healerPosition = healer.transform.position;
            float sqrDistance = Vector3.SqrMagnitude(healerPosition - target.transform.position);

            if(lowestSqrDistance > sqrDistance)
            {
                lowestSqrDistance = sqrDistance;
                nearestHealer = healer;
            }
        }

        if (nearestHealer)
        {
            nearestHealer.Target = target;
            return true;
        }

        return false;
    }

    public bool AssignSupportTo(Unit target)
    {
        float lowestSqrDistance = float.MaxValue;
        SupportUnit nearestSupport = null;

        //  Don't need support anymore
        if (target.agent.Agressor == null) return true;

        foreach (Unit unit in units)
        {
            if (unit == target) continue;

            SupportUnit support = unit as SupportUnit;

            if (support == null || support.Target == leader) continue;

            Vector3 coverPosition = support.GetCoverPosition(target.transform.position, target.agent.Agressor.transform.position);

            float sqrDistance = Vector3.SqrMagnitude(support.transform.position - coverPosition);

            if (lowestSqrDistance > sqrDistance)
            {
                lowestSqrDistance = sqrDistance;
                nearestSupport = support;
            }
        }

        if (nearestSupport)
        {
            nearestSupport.Target = target;
            return true;
        }

        return false;
    }

    public void ReceiveAlert(Unit fromUnit)
    {
        foreach (Unit unit in units)
        {
            if (unit.agent.Agressor) continue;

            if (Vector3.SqrMagnitude(unit.transform.position - fromUnit.transform.position) < alertMaxDistance * alertMaxDistance) continue;

            //  One for all, all for one
            unit.agent.Agressor = fromUnit.agent.Agressor;
        }
    }

    public void UpdatePositions()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];

            if (unit && (!unit.gameObject.activeInHierarchy || unit.HasDuty())) continue;

            Vector3 pos = ComputeUnitPosition(i);
            unit.movement.MoveTo(pos);
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

    #endregion
}
