using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour, IDamageable
{
    [SerializeField]
    float BulletPower = 1000f;
    [SerializeField]
    private GameObject BulletPrefab;
    protected Transform GunTransform;

    [SerializeField]
    protected int MaxHP = 100;
    protected bool IsDead = false;

    protected int CurrentHP;

    public void AddDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            IsDead = true;
            CurrentHP = 0;

            OnDeath();
        }

        OnHealthChange();
    }

    protected virtual void OnHealthChange() { }
    protected virtual void OnDeath() { }

    public virtual void ShootToPosition(Vector3 pos)
    {
        // instantiate bullet
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);
        }
    }
}
