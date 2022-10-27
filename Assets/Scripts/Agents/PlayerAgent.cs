using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerAgent : Agent
{
    [SerializeField] private GameObject m_targetCursorPrefab = null;
    [SerializeField] private GameObject m_NPCTargetCursorPrefab = null;

    private GameObject m_targetCursor = null;
    private GameObject m_NPCTargetCursor = null;

    [HideInInspector] public UnitLeader leader = null;

    void Start()
    {
        m_gunTransform = transform.Find("Gun");

        m_currentHealth = m_maxHealth;
    }
    private new void Awake()
    {
        base.Awake();

        leader = GetComponent<UnitLeader>();
    }

    private GameObject GetTargetCursor()
    {
        if (m_targetCursor == null)
            m_targetCursor = Instantiate(m_targetCursorPrefab);

        return m_targetCursor;
    }

    public void AimAtPosition(Vector3 pos)
    {
        GetTargetCursor().transform.position = pos;
        if (Vector3.Distance(transform.position, pos) > 2.5f)
        {
            Vector3 newForward = pos - transform.position;
            newForward.y = 0f;

            transform.forward = newForward.normalized;
        }
    }

    public void NPCShootToPosition(Vector3 pos)
    {
        StartCoroutine(InstantiateTarget(pos, 5f));
    }

    public void Shoot(Vector3 pos)
    {
        ShootForward();
        StartCoroutine(InstantiateTarget(pos, 0.5f));
    }

    protected override void OnHealthChange()
    {
        base.OnHealthChange();
    }

    public void MoveToward(Vector3 velocity)
    {
    }

    IEnumerator InstantiateTarget(Vector3 position, float time)
    {
        if (m_NPCTargetCursor) Destroy(m_NPCTargetCursor);

        GameObject target = Instantiate(m_NPCTargetCursorPrefab);
        target.transform.position = position;

        UI_CircleSlider circle = target.GetComponentInChildren<UI_CircleSlider>();

        m_NPCTargetCursor = target;

        leader?.m_Squad?.SetTarget(target);

        float startTime = Time.time;


        while(time > Time.time - startTime)
        {
            circle?.SetFillAmount(1f - (Time.time - startTime) / time);

            yield return null;
        }

        if(target) Destroy(target);
    }

    public float IsAttacked()
    {
        return Convert.ToSingle(agressor != null);
    }
}
