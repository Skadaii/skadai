using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UnitSquad))]
public class SquadController : MonoBehaviour
{
    #region Internal Classes

    [System.Serializable]
    private class UnitType
    {
        public GameObject prefab;
        public int count;
    }

    #endregion


    #region Variables

    [SerializeField] private List<UnitType> unitTypes = new List<UnitType>();

    private UnitSquad m_Squad = null;
    private int m_unitCount = 0;

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        m_Squad = GetComponent<UnitSquad>();
    }

    private void Start()
    {
        unitTypes.ForEach(type => { m_unitCount += type.count; });
        
        if (m_unitCount != 0) InitializeUnits();
    }

    #endregion


    #region Functions

    private void InitializeUnits()
    {
        List<Unit> unitList = new List<Unit>();
        Transform startTransform = (m_Squad.leader ?? this as Component).transform;

        float spawnAngle = Mathf.PI * 2.0f / m_unitCount;
        int   currentCount = 0;

        foreach(UnitType type in unitTypes)
        {
            for (int i = 0; i < type.count; i++)
            {
                Vector3 spawnPos;

                if (m_Squad.formation)
                {
                    spawnPos = m_Squad.formation.ComputePosition(startTransform.position, startTransform.rotation, currentCount);
                }
                else
                {
                    float currentAngle = spawnAngle * currentCount;
                    spawnPos = startTransform.position + (Vector3.right * Mathf.Cos(currentAngle) + Vector3.forward * Mathf.Sin(currentAngle)) * 2.0f;
                }

                Unit unit = Instantiate(type.prefab, spawnPos, startTransform.rotation).GetComponent<Unit>();

                unitList.Add(unit);

                currentCount++;
            }
        }

        m_Squad.Units = unitList;
    }

    #endregion
}
