using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAgent : AIAgent
{

    protected override void OnDeath()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        GunTransform = transform.Find("Body/Gun");
        if (GunTransform == null)
            Debug.Log("could not find gun transform");

        CurrentHP = MaxHP;
    }


    private void Update()
    {
        base.Update();

        CheckTarget();
    }



    private void CheckTarget()
    {
        float minDistance = float.MaxValue;

        Target = null;

        AgentTrespassers.ForEach(agent =>
        {
            float distance = Vector3.SqrMagnitude(transform.position - agent.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                Target = agent.gameObject;
            }
        });
    }
}
