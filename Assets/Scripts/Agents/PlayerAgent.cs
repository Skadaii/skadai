using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAgent : Agent
{
    [SerializeField]
    GameObject TargetCursorPrefab = null;
    [SerializeField]
    GameObject NPCTargetCursorPrefab = null;

    GameObject TargetCursor = null;
    GameObject NPCTargetCursor = null;

    public UnitLeader leader = null;

    private new void Awake()
    {
        base.Awake();

        leader = GetComponent<UnitLeader>();
    }

    private GameObject GetTargetCursor()
    {
        if (TargetCursor == null)
            TargetCursor = Instantiate(TargetCursorPrefab);

        return TargetCursor;
    }
    private GameObject GetNPCTargetCursor()
    {
        if (NPCTargetCursor == null)
            NPCTargetCursor = Instantiate(NPCTargetCursorPrefab);

        return NPCTargetCursor;
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

    /*public void NPCShootToPosition(Vector3 pos)
    {
        GetNPCTargetCursor().transform.position = pos;

        leader.m_Squad.ShootToPosition(pos);
    }*/

    public void NPCShootToPosition(Vector3 pos)
    {
        GetNPCTargetCursor().transform.position = pos;

        leader.m_Squad.SetTarget(GetNPCTargetCursor());
    }

    protected override void OnHealthChange()
    {
        base.OnHealthChange();
    }

    public void MoveToward(Vector3 velocity)
    {
    }

    #region MonoBehaviour Methods
    void Start()
    {
        GunTransform = transform.Find("Gun");

        CurrentHP = MaxHP;
    }

    #endregion

}
