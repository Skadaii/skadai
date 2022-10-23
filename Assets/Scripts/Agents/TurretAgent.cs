using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAgent : AIAgent
{
    void Start()
    {
        GunTransform = transform.Find("Body/Gun");
        if (GunTransform == null)
            Debug.Log("could not find gun transform");

        CurrentHP = MaxHP;
    }


    private new void Update()
    {
        base.Update();

        CheckTarget();
    }



    private void CheckTarget()
    {
        float minDistance = float.MaxValue;

        GameObject newTarget = null;

        AgentTrespassers.ForEach(agent =>
        {
            float distance = Vector3.SqrMagnitude(transform.position - agent.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                newTarget = agent.gameObject;
            }
        });

        SetTarget(newTarget);
    }
}
