using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    private GameObject m_hitFX;
    private Agent m_shooter = null;

    public float life = 2f;
    [SerializeField] private float m_impactShakeMaxRadius = 10f;
    [SerializeField] private float m_impactShakeScale = 0.1f;

    private MeshRenderer m_meshRenderer;
    private int m_damages = 10;
    private Vector3 m_velocity;

    private int m_ignoreMaskLayer; 

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        m_hitFX = Resources.Load("FXs/ParticleHit") as GameObject;

        m_meshRenderer = GetComponent<MeshRenderer>();

        m_ignoreMaskLayer = ~LayerMask.GetMask("Entity");
    }

    void Start()
    {
        Destroy(gameObject, life);
    }

    private void FixedUpdate()
    {
        if (Raycast(out RaycastHit hit))
        {
            Agent agent = hit.collider.gameObject.GetComponentInParent<Agent>();

            if (agent || hit.collider.gameObject.TryGetComponent(out agent))
            {
                if (agent.AgentTeam == m_shooter.AgentTeam)
                {
                    //  Proceed with another check with layer mask of entities to avoid wall through passing
                    if (!Raycast(out hit, m_ignoreMaskLayer))
                    {
                        UpdatePos();
                        return;
                    }
                }
                else
                {
                    //  If the hitted collider is a damage collider then apply basic damage else apply reduced damages (for tanks)
                    bool dealHeavyDamage = agent.DamageCollider == hit.collider;
                    agent.AddDamage(dealHeavyDamage ? m_damages : m_damages / 2, m_shooter, hit, dealHeavyDamage);
                }
            }

            OnHitEffects(hit);
            return;
        }

        UpdatePos();
    }

    #endregion


    #region Functions

    private bool Raycast(out RaycastHit hit, int layerMask = -1)
    {
        return Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, m_velocity.normalized, out hit, m_velocity.magnitude * Time.fixedDeltaTime, layerMask, QueryTriggerInteraction.Ignore);
    }

    private void UpdatePos()
    {
        transform.position += m_velocity * Time.fixedDeltaTime;
    }

    private void OnHitEffects(RaycastHit hit)
    {
        CameraController cam = GameInstance.Instance.playerCamera;

        float distFactor = 1f - Mathf.Clamp(Vector3.Distance(cam.Target.position, hit.point) / m_impactShakeMaxRadius, 0f, 1f);
        float shakeStrength = m_impactShakeScale * distFactor;

        cam.Shake(shakeStrength, 0.15f);

        GameObject hitParticles = Instantiate(m_hitFX, null);

        hitParticles.transform.position = hit.point;
        hitParticles.transform.forward  = hit.normal;

        m_meshRenderer.enabled = false;
        this.enabled      = false;

        transform.position = hit.point;
        Destroy(gameObject, 2f);
        Destroy(hitParticles, 2f);
    }

    public void Shoot(Agent shooter, Vector3 direction, float force, int damages)
    {
        m_velocity = direction * force;
        m_shooter  = shooter;
        m_damages  = damages;
    }

    #endregion
}
