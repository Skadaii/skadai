using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCController : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    private NPCMovement m_Movement = null;

    void Awake()
    {
        m_Movement = GetComponent<NPCMovement>();
    }

    private void Update()
    {
    }
}
