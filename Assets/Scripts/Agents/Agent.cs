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

    [SerializeField] private float HPBarHeight = 1.5f;
    [SerializeField] private   GameObject   HPBarPrefab;
    private UI_HealthBar HPBar;

    private GameObject ExplodeFX;
    public  GameObject Assaillant;

    Coroutine AttackedStateCoroutine;

    protected void Awake()
    {
        ExplodeFX = Resources.Load("FXs/ParticleExplode") as GameObject;
    }

    private void OnEnable()
    {
        CurrentHP = MaxHP;
        IsDead    = false;

        if (HPBarPrefab != null)
        {
            GameObject go = Instantiate(HPBarPrefab, transform);

            go.transform.localPosition = Vector3.up * HPBarHeight;

            HPBar = go.GetComponent<UI_HealthBar>();
        }
    }

    private void OnDisable()
    {
        if (HPBar != null) HPBar.Destroy();

        if (AttackedStateCoroutine != null)
        {
            StopCoroutine(AttackedStateCoroutine);
            AttackedStateCoroutine = null;
        }
    }

    public void AddDamage(int amount, GameObject attacker)
    {
        Assaillant = attacker;

        if (AttackedStateCoroutine != null)
        {
            StopCoroutine(AttackedStateCoroutine);
            AttackedStateCoroutine = null;
        }

        AttackedStateCoroutine = StartCoroutine(AttackedStateCooldown(3f));

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

    public bool AddHealth(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);

        OnHealthChange();

        return CurrentHP >= MaxHP;
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
        if (BulletPrefab && !GunCheckObstacle())
        {
            GameObject bullet = Instantiate(BulletPrefab, GunTransform.position, Quaternion.identity);

            if (bullet.TryGetComponent(out Bullet bulletComp))
            {
                bulletComp.IgnoreMask = ~ (1 << gameObject.layer);
                bulletComp.Shooter = gameObject;
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);

        }
    }

    protected bool GunCheckObstacle()
    {
        Vector3 start = GunTransform.position - GunTransform.up * GunTransform.lossyScale.y;
        Vector3 end = GunTransform.position + GunTransform.up * GunTransform.lossyScale.y;
        Ray ray = new Ray(start, end - start);

        return Physics.SphereCast(ray, GunTransform.lossyScale.x * 0.5f, Vector3.Distance(start, end), ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore);
    }

    IEnumerator AttackedStateCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        Assaillant = null;

        AttackedStateCoroutine = null;
    }

    public float GetLifePercent()
    {
        return (float)CurrentHP / (float)MaxHP;
    }
}
