using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAgent : Agent
{
    [SerializeField]
    float BulletPower = 1000f;
    [SerializeField]
    GameObject BulletPrefab;

    [SerializeField]
    GameObject TargetCursorPrefab = null;
    [SerializeField]
    GameObject NPCTargetCursorPrefab = null;

    [SerializeField]
    Slider HPSlider = null;

    GameObject TargetCursor = null;
    GameObject NPCTargetCursor = null;
    Transform GunTransform;

    private GameObject GetTargetCursor()
    {
        if (TargetCursor == null)
            TargetCursor = Instantiate(TargetCursorPrefab);
        return TargetCursor;
    }
    private GameObject GetNPCTargetCursor()
    {
        if (NPCTargetCursor == null)
        {
            NPCTargetCursor = Instantiate(NPCTargetCursorPrefab);
        }
        return NPCTargetCursor;
    }
    public void AimAtPosition(Vector3 pos)
    {
        GetTargetCursor().transform.position = pos;
        if (Vector3.Distance(transform.position, pos) > 2.5f)
            transform.LookAt(pos + Vector3.up * transform.position.y);
    }
    public void ShootToPosition(Vector3 pos)
    {
        // instantiate bullet
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate<GameObject>(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);
        }
    }
    public void NPCShootToPosition(Vector3 pos)
    {
        GetNPCTargetCursor().transform.position = pos;
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
        CurrentHP = MaxHP;
        GunTransform = transform.Find("Gun");

        if (HPSlider != null)
        {
            HPSlider.maxValue = MaxHP;
            HPSlider.value = CurrentHP;
        }
    }
    void Update()
    {
        
    }

    #endregion

}
