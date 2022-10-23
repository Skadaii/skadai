using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected float BulletPower = 1000f;
    [SerializeField]
    protected GameObject BulletPrefab;
    protected Transform GunTransform;

    [SerializeField]
    protected int MaxHP = 100;
    protected bool IsDead = false;

    protected int CurrentHP;

    protected Vector3 DeltaVel = Vector3.zero;

    [SerializeField]
    protected float ShootFrequency = 1f;

    protected float NextShootDate = 0f;

    public UnityEvent<GameObject> OnHit = new UnityEvent<GameObject>();
    public void AddDamage(int amount, GameObject attacker)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            IsDead = true;
            CurrentHP = 0;

            OnDeath();
        }
        else
            OnHit.Invoke(attacker);

        OnHealthChange();
    }
    public void AddHealth(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);

        OnHealthChange();
    }

    protected virtual void OnHealthChange() { }
    protected virtual void OnDeath() { }

    public virtual void ShootForward()
    {
        // instantiate bullet
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);

            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.Shooter = gameObject;
        }
    }
}
