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

    //public void OnLeaderNeedHeal()
    //{
    //    float minDist = Mathf.Infinity;

    //    Vector3 dir = Vector3.zero;

    //    Get the nearest unit to the line leader/ attacker
    //    for (int i = 0; i < units.Count; i++)
    //    {
    //        Unit unit = units[i];

    //        if (!unit.gameObject.activeInHierarchy || unit.speciality != UnitSpeciality.Healer || defender == unit) continue;

    //        dir = leader.transform.position - unit.transform.position;

    //        float dist = Vector3.SqrMagnitude(dir);

    //        if (dist < minDist * minDist)
    //        {
    //            healer = unit;
    //            minDist = dist;
    //        }
    //    }

    //    if (healer == null) return;

    //    healer.movement.MoveTo(leader.transform.position + dir.normalized * 2f);

    //    Debug.Log("Healing !");
    //}

    //public void OnLeaderNeedCover(Vector3 position)
    //{
    //    float minDist = Mathf.Infinity;

    //    Get the nearest unit to the line leader/ attacker
    //    for (int i = 0; i < units.Count; i++)
    //    {
    //        Unit unit = units[i];

    //        if (!unit.gameObject.activeInHierarchy) continue;

    //        float dist = Vector3.Distance(unit.transform.position, position);
    //        if (dist < minDist)
    //        {
    //            defender = unit;
    //            minDist = dist;
    //        }
    //    }

    //    defender.movement.MoveTo(position);
    //}

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
