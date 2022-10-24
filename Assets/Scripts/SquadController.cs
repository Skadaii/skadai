using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UnitSquad))]
public class SquadController : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private int unitCount = 3;

    private UnitSquad m_Squad = null;

    private void Awake()
    {
        m_Squad = GetComponent<UnitSquad>();
    }

    private void Start()
    {
        if (unitPrefab)
            InitializeUnits();
    }

    private void InitializeUnits()
    {
        List<Unit> unitList = new List<Unit>();
        Transform startTransform = (m_Squad.leader ?? this as Component).transform;

        float spawnAngle = Mathf.PI * 2.0f / unitCount;
        for (int i = 0; i < unitCount; i++)
        {
            Vector3 spawnPos;

            if (m_Squad.formation)
            {
                spawnPos = m_Squad.formation.ComputePosition(startTransform.position, startTransform.rotation, i);
            }
            else
            {
                float currentAngle = spawnAngle * i;
                spawnPos = startTransform.position + (Vector3.right * Mathf.Cos(currentAngle) + Vector3.forward * Mathf.Sin(currentAngle)) * 2.0f;
            }

            GameObject unitInst = Instantiate(unitPrefab, spawnPos, startTransform.rotation);
            Unit unit = unitInst.GetComponent<Unit>();

            unitList.Add(unit);
        }

        m_Squad.Units = unitList;
    }
}
