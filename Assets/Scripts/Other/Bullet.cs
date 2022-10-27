using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject m_hitFX;
    private Agent m_shooter = null;

    private Rigidbody m_rigidBody;

    public float Duration = 2f;
    [SerializeField] private float m_impactShakeMaxRadius = 10f;
    [SerializeField] private float m_impactShakeScale = 0.1f;

    private MeshRenderer MRenderer;

    private void Awake()
    {
        m_hitFX = Resources.Load("FXs/ParticleHit") as GameObject;
        m_rigidBody = GetComponent<Rigidbody>();

        MRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        Destroy(gameObject, Duration);
    }

    private void Update()
    {
        if(Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, m_rigidBody.velocity.normalized, out RaycastHit hit, m_rigidBody.velocity.magnitude * Time.fixedDeltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.TryGetComponent(out Agent agent) && agent.team == m_shooter.team) return;

            IDamageable damagedAgent = hit.collider.gameObject.GetComponentInParent<IDamageable>();

            if (damagedAgent == null) damagedAgent = hit.collider.gameObject.GetComponent<IDamageable>();

            damagedAgent?.AddDamage(10, m_shooter);

            OnHitEffects(hit);
        }
    }

    private void OnHitEffects(RaycastHit hit)
    {
        CameraController cam = GameInstance.Instance.playerCamera;

        float distFactor = 1f - Mathf.Clamp(Vector3.Distance(cam.Target.position, hit.point) / m_impactShakeMaxRadius, 0f, 1f);
        Debug.Log(m_impactShakeMaxRadius + "    " + distFactor);
        float shakeStrength = m_impactShakeScale * distFactor;

        cam.Shake(shakeStrength, 0.15f);

        GameObject hitParticles = Instantiate(m_hitFX, null);

        hitParticles.transform.position = hit.point;
        hitParticles.transform.forward = hit.normal;

        MRenderer.enabled = false;
        this.enabled      = false;

        m_rigidBody.isKinematic = true;

        Destroy(gameObject, 2f);
        Destroy(hitParticles, 2f);
    }

    public void Shoot(Agent shooter, Vector3 direction, float force)
    {
        m_shooter = shooter;
        m_rigidBody.AddForce(direction * force);
    }
}
