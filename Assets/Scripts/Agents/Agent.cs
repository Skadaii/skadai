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

    [SerializeField] private float HPBarHeight = 1.5f;
    [SerializeField] private   GameObject   HPBarPrefab;
    private UI_HealthBar HPBar;

    private GameObject ExplodeFX;

    protected void Awake()
    {
        ExplodeFX = Resources.Load("FXs/ParticleExplode") as GameObject;
    }

    private void OnEnable()
    {
        if (HPBarPrefab != null)
        {
            GameObject go = Instantiate(HPBarPrefab, transform);

            go.transform.localPosition = Vector3.up * HPBarHeight;

            HPBar = go.GetComponent<UI_HealthBar>();
        }
    }

    private void OnDisable()
    {
        if(HPBar != null) HPBar.Destroy();

    }


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
    public void AddHealth(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);

        OnHealthChange();
    }

    protected virtual void OnHealthChange() 
    {
        HPBar?.SetHealthPercentage((float)CurrentHP / (float)MaxHP);
    }

    protected virtual void OnDeath()
    {
        GameObject explodeParticles = Instantiate(ExplodeFX, null);

        explodeParticles.transform.position = transform.position;

        Destroy(explodeParticles, 2f);
    }

    public virtual void ShootForward()
    {
        // instantiate bullet
        if (BulletPrefab)
        {
            if (Physics.Raycast(GunTransform.position, GunTransform.position + GunTransform.up, GunTransform.lossyScale.y)) return;

            GameObject bullet = Instantiate(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);
        }
    }
}
