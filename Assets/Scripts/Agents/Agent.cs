using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour, IDamageable
{
    #region Variables

    [Header("Health parameters")]
    [SerializeField] protected int m_maxHealth = 100;
    [SerializeField] private float m_healthBarYpos = 1.5f;
    [SerializeField] private GameObject m_HealthBarPrefab;

    [Header("Shoot & bullet parameters")]
    [SerializeField] protected float m_shootFrequency = 1f;
    [SerializeField] protected GameObject m_bulletPrefab;
    [SerializeField] protected float m_bulletPower = 100f;
    [SerializeField] protected int   m_bulletDamage = 10;

    [Header("Team parameters")]

    [SerializeField] protected ScriptableTeam m_agentTeam;

    //  Other variables

    protected int m_currentHealth = 100;
    protected Transform m_gunTransform;
    protected float m_nextShootDate = 0f;

    private Material m_materialInstance;

    // Attacked info variables

    protected Agent m_agressor;
    [HideInInspector] public UnityEvent OnHit = new UnityEvent();

    //  HealthBar variables

    private UI_HealthBar m_healthBar;


    //  Explosion FX variables

    private float m_explosionShakeScale = 0.15f;
    private float m_explosionShakeDuration = 0.25f;
    protected GameObject m_explosionFX;

    float m_considerBeingAttackedFor = 3f;
    float m_lastAttackedTime = 0f;

    #endregion


    #region properties

    public CapsuleCollider DamageCollider { get; private set; }
    public GameObject HurtFX { get; protected set; }

    public int CurrentHealth { get { return m_currentHealth; } }

    public Agent Agressor
    {
        get { return m_agressor; }

        set
        {
            m_agressor = value;

            if (m_agressor)
            {
                m_lastAttackedTime = Time.time;
            }
        }
    }

    public ScriptableTeam AgentTeam
    {
        get { return m_agentTeam; }

        set 
        { 
            m_agentTeam = value;

            if (m_materialInstance && m_agentTeam)
            {
                m_materialInstance.color = m_agentTeam.teamColor;
            }
        }
    }

    #endregion


    #region MonoBehaviour

    protected void Awake()
    {
        DamageCollider = GetComponent<CapsuleCollider>();
    
        //  Search for Renderer to get the material instance

        Renderer rend = transform.Find("Body").GetComponent<Renderer>();
        m_materialInstance = rend.material;

        if (m_materialInstance && m_agentTeam)
        {
            m_materialInstance.color = m_agentTeam.teamColor;
        }

        //  Search for gun transform

        m_gunTransform = transform.Find("Body/Gun");

        if (m_gunTransform == null) Debug.LogWarning("could not find gun transform");
    }

    protected void Start()
    {
        m_currentHealth = m_maxHealth;
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
    }

    protected void Update()
    {
        if(m_agressor != null && Time.time - m_lastAttackedTime >= m_considerBeingAttackedFor)
        {
            m_agressor = null;
        }
    }

    #endregion


    #region Functions

    public void AddDamage(int damage, Agent attacker, RaycastHit hit, bool playFX = false)
    {
        //  Important to call Agressor property set here
        Agressor = attacker;

        m_lastAttackedTime = Time.time;

        m_currentHealth -= damage;

        if(playFX && HurtFX)
        {
            GameObject hitParticles = Instantiate(HurtFX, transform.parent);

            hitParticles.transform.position = hit.point;
            hitParticles.transform.forward = hit.normal;

            Destroy(hitParticles, 2f);
        }

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

        GameInstance.Instance.playerCamera?.Shake(m_explosionShakeScale, m_explosionShakeDuration);
    }

    public virtual void ShootForward()
    {
        // instantiate bullet
        if (m_bulletPrefab && !GunCheckObstacle())
        {
            GameObject bullet = Instantiate(m_bulletPrefab, m_gunTransform.position, Quaternion.identity);

            if (bullet.TryGetComponent(out Bullet bulletComp))
            {
                bulletComp.Shoot(this, GetBulletTrajectory(), m_bulletPower, m_bulletDamage);
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

    public float GetLifePercent()
    {
        return (float)CurrentHealth / (float)m_maxHealth;
    }

    protected virtual Vector3 GetBulletTrajectory()
    {
        return transform.forward;
    }

    #endregion
}
