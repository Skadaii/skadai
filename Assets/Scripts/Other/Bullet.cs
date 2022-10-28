using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    private GameObject m_hitFX;
    private Agent m_shooter = null;

    private Rigidbody m_rigidBody;

    public float life = 2f;
    [SerializeField] private float m_impactShakeMaxRadius = 10f;
    [SerializeField] private float m_impactShakeScale = 0.1f;

    private MeshRenderer m_meshRenderer;
    private int m_damages = 10;

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        m_hitFX = Resources.Load("FXs/ParticleHit") as GameObject;
        m_rigidBody = GetComponent<Rigidbody>();

        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        Destroy(gameObject, life);
    }

    private void Update()
    {
        if(Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, m_rigidBody.velocity.normalized, out RaycastHit hit, m_rigidBody.velocity.magnitude * Time.fixedDeltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.TryGetComponent(out Agent agent) && agent.AgentTeam == m_shooter.AgentTeam) return;

            if(hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                //  If the hitted collider is a damage collider then apply basic damage else apply reduced damages (for tanks)
                damageable.AddDamage(damageable.DamageCollider == hit.collider ? m_damages : m_damages/2, m_shooter);
            }

            OnHitEffects(hit);
        }
    }

    #endregion


    #region Functions

    private void OnHitEffects(RaycastHit hit)
    {
        CameraController cam = GameInstance.Instance.playerCamera;

        float distFactor = 1f - Mathf.Clamp(Vector3.Distance(cam.Target.position, hit.point) / m_impactShakeMaxRadius, 0f, 1f);
        float shakeStrength = m_impactShakeScale * distFactor;

        cam.Shake(shakeStrength, 0.15f);

        GameObject hitParticles = Instantiate(m_hitFX, null);

        hitParticles.transform.position = hit.point;
        hitParticles.transform.forward = hit.normal;

        m_meshRenderer.enabled = false;
        this.enabled      = false;

        m_rigidBody.isKinematic = true;

        Destroy(gameObject, 2f);
        Destroy(hitParticles, 2f);
    }

    public void Shoot(Agent shooter, Vector3 direction, float force, int damages)
    {
        m_shooter = shooter;
        m_rigidBody.AddForce(direction * force);
        m_damages = damages;
    }

    #endregion
}
