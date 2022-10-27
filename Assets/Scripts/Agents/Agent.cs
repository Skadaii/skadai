using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour, IDamageable
{
    public int team;

    [SerializeField]
    protected float m_bulletPower = 1000f;
    [SerializeField]
    protected GameObject m_bulletPrefab;
    protected Transform m_gunTransform;

    [SerializeField]
    protected int m_maxHealth = 100;
    protected int m_currentHealth = 100;

    [SerializeField]
    protected float m_shootFrequency = 1f;

    protected float m_nextShootDate = 0f;
    
    public UnityEvent OnHit = new UnityEvent();

    [SerializeField] private float m_healthBarYpos = 1.5f;
    [SerializeField] private GameObject m_HealthBarPrefab;
    private UI_HealthBar m_healthBar;

    private float explosionShakeScale = 0.15f;
    private float explosionShakeDuration = 0.25f;
    private GameObject m_explosionFX;
    public  Agent agressor;

    private Coroutine m_attackedStateCoroutine;

    public int CurrentHealth { get { return m_currentHealth; } }

    protected void Awake()
    {
        m_explosionFX = Resources.Load("FXs/ParticleExplode") as GameObject;
    }

    protected void OnEnable()
    {
        m_currentHealth = m_maxHealth;

        if (m_HealthBarPrefab != null)
        {
            GameObject go = Instantiate(m_HealthBarPrefab, transform);

            go.transform.localPosition = Vector3.up * m_healthBarYpos;

            m_healthBar = go.GetComponent<UI_HealthBar>();
        }
    }

    protected void OnDisable()
    {
        if (m_healthBar != null) m_healthBar.Destroy();

        if (m_attackedStateCoroutine != null)
        {
            StopCoroutine(m_attackedStateCoroutine);
            m_attackedStateCoroutine = null;
        }
    }

    public void AddDamage(int amount, Agent attacker)
    {
        agressor = attacker;

        if (m_attackedStateCoroutine != null)
        {
            StopCoroutine(m_attackedStateCoroutine);
            m_attackedStateCoroutine = null;
        }

        m_attackedStateCoroutine = StartCoroutine(AttackedStateCooldown(3f));

        m_currentHealth -= amount;
        if (m_currentHealth <= 0)
        {
            m_currentHealth = 0;

            OnDeath();
        }
        
        OnHit.Invoke();

        OnHealthChange();
    }

    public bool AddHealth(int amount)
    {
        m_currentHealth = Mathf.Min(m_currentHealth + amount, m_maxHealth);

        OnHealthChange();

        return m_currentHealth >= m_maxHealth;
    }

    protected virtual void OnHealthChange() 
    {
        m_healthBar?.SetHealthPercentage((float)CurrentHealth / (float)m_maxHealth);
    }

    protected virtual void OnDeath()
    {
        GameObject explodeParticles = Instantiate(m_explosionFX, null);

        explodeParticles.transform.position = transform.position;

        Destroy(explodeParticles, 2f);

        GameInstance.Instance.playerCamera?.Shake(explosionShakeScale, explosionShakeDuration);
    }

    public virtual void ShootForward()
    {
        // instantiate bullet
        if (m_bulletPrefab && !GunCheckObstacle())
        {
            GameObject bullet = Instantiate(m_bulletPrefab, m_gunTransform.position, Quaternion.identity);

            if (bullet.TryGetComponent(out Bullet bulletComp))
            {
                bulletComp.Shoot(this, GetBulletTrajectory(), m_bulletPower);
            }
        }
    }

    protected bool GunCheckObstacle()
    {
        Vector3 start = m_gunTransform.position - m_gunTransform.up * m_gunTransform.lossyScale.y;
        Vector3 end = m_gunTransform.position + m_gunTransform.up * m_gunTransform.lossyScale.y;
        Ray ray = new Ray(start, end - start);

        return Physics.SphereCast(ray, m_gunTransform.lossyScale.x * 0.5f, Vector3.Distance(start, end), ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore);
    }

    IEnumerator AttackedStateCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        agressor = null;

        m_attackedStateCoroutine = null;
    }

    public float GetLifePercent()
    {
        return (float)CurrentHealth / (float)m_maxHealth;
    }

    protected virtual Vector3 GetBulletTrajectory()
    {
        return transform.forward;
    }
}
