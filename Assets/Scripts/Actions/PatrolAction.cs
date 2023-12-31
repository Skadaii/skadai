using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : MonoBehaviour
{
    [SerializeField] public Transform[] patrolPoints = null;
    private int index = 0;

    Movement movement = null;

    [SerializeField] private float epsilon = 0.1f;

    public float GetPatrolState()
    {
        return 1f;
    }

    public void Patrol()
    {
        if (movement.HasReachedPos(epsilon))
        {
            index = (index + 1) % patrolPoints.Length;
            movement.MoveTo(patrolPoints[index]);
        }
    }

    public void Awake()
    {
        movement = GetComponent<Movement>();
    }
    public void Start()
    {
        index = 0;
        movement.MoveTo(patrolPoints[index]);
    }

    private void Update()
    {
        Patrol();
    }
}