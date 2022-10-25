using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerAgent : Agent
{
    [SerializeField]
    GameObject TargetCursorPrefab = null;
    [SerializeField]
    GameObject NPCTargetCursorPrefab = null;

    GameObject TargetCursor = null;
    GameObject NPCTargetCursor = null;

    public UnitLeader leader = null;
    Rigidbody rb;
    void Start()
    {
        GunTransform = transform.Find("Gun");

        CurrentHP = MaxHP;
    }
    private new void Awake()
    {
        base.Awake();

        leader = GetComponent<UnitLeader>();
        rb = GetComponent<Rigidbody>();
    }

    private GameObject GetTargetCursor()
    {
        if (TargetCursor == null)
            TargetCursor = Instantiate(TargetCursorPrefab);

        return TargetCursor;
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

    protected override void OnHealthChange()
    {
        base.OnHealthChange();
    }

    public void MoveToward(Vector3 velocity)
    {
    }

    IEnumerator InstantiateTarget(Vector3 position, float time)
    {
        if (NPCTargetCursor) Destroy(NPCTargetCursor);

        GameObject target = Instantiate(NPCTargetCursorPrefab);
        target.transform.position = position;

        UI_CircleSlider circle = target.GetComponentInChildren<UI_CircleSlider>();

        NPCTargetCursor = target;

        leader?.m_Squad?.SetTarget(target);

        float startTime = Time.time;


        while(time > Time.time - startTime)
        {
            circle?.SetFillAmount(1f - (Time.time - startTime) / time);

            yield return null;
        }

        if(target) Destroy(target);

        //leader.m_Squad.SetTarget(null);
    }

    public float IsAttacked()
    {
        return Convert.ToSingle(Assaillant != null);
    }
}
