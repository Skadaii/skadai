using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
    public Movement movement = null;
    public Agent agent = null;

    [SerializeField] public UnitSquad m_Squad = null;

    // Start is called before the first frame update
    void Awake()
    {
        movement = GetComponent<Movement>();
        agent = GetComponent<Agent>();
    }
}
