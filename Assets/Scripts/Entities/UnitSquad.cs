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
            unitPositions = new Vector3[units.Count];
        }
    }

    private void Start()
    {
        // If leader is null, set a virtual one
        leader ??= CreateVirtuaLeader("Leader");
    }

    private void Update()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = ComputeUnitPosition(i);
            units[i].movement.MoveTo(pos);
            unitPositions[i] = pos;
        }
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
        GameObject leaderGO = new GameObject();
        //leaderGO.AddComponent<PathfinderAStar>();
        //leaderGO.AddComponent<FollowPath>();
        leaderGO.AddComponent<Movement>();

        return leaderGO.AddComponent<UnitLeader>();
    }
}
