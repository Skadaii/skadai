using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSquad : MonoBehaviour
{
    public UnitLeader leader = null;

    private void Start()
    {
        // If leader is null, set a virtual one
        leader ??= CreateVirtuaLeader("Leader");
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
