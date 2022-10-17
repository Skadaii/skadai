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

    [SerializeField]
    Slider HPSlider = null;

    GameObject TargetCursor = null;
    GameObject NPCTargetCursor = null;

    public UnitLeader leader = null;

    private void Awake()
    {
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
            transform.LookAt(pos + Vector3.up * transform.position.y);
    }

    public void NPCShootToPosition(Vector3 pos)
    {
        GetNPCTargetCursor().transform.position = pos;

        leader.m_Squad.ShootToPosition(pos);
    }

    protected override void OnHealthChange()
    {
        if (HPSlider != null)
        {
            HPSlider.value = CurrentHP;
        }
    }

    public void MoveToward(Vector3 velocity)
    {
    }

    #region MonoBehaviour Methods
    void Start()
    {
        GunTransform = transform.Find("Gun");

        CurrentHP = MaxHP;

        if (HPSlider != null)
        {
            HPSlider.maxValue = MaxHP;
            HPSlider.value = CurrentHP;
        }
    }

    #endregion

}
