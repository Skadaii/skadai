using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : MonoBehaviour
{
    [SerializeField] public Transform[] patrolPoints = null;
    private int index = 0;

    Movement movement = null;

    [SerializeField] private float epsilon = 0.5f;

    public void Awake()
    {
        movement = GetComponent<Movement>();
    }
    public void Start()
    {
        index = 0;
        movement.MoveTo(patrolPoints[index].position);
    }
    public void Update()
    {
        if (movement.HasReachedPos(epsilon))
        {
            index = (index + 1) % patrolPoints.Length;
            movement.MoveTo(patrolPoints[index].position);
        }
    }
}