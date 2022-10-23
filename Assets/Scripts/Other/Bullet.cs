using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject HitFX;
    Rigidbody RB;

    public float Duration = 2f;
    public LayerMask IgnoreMask;
    private MeshRenderer MRenderer;

    private void Awake()
    {
        HitFX = Resources.Load("FXs/ParticleHit") as GameObject;
        RB = GetComponent<Rigidbody>();

        MRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        Destroy(gameObject, Duration);
    }

    private void Update()
    {
        if(Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, RB.velocity.normalized, out RaycastHit hit, RB.velocity.magnitude * Time.fixedDeltaTime, IgnoreMask, QueryTriggerInteraction.Ignore))
        {
            IDamageable damagedAgent = hit.collider.gameObject.GetComponentInParent<IDamageable>();

            if (damagedAgent == null) damagedAgent = hit.collider.gameObject.GetComponent<IDamageable>();

            damagedAgent?.AddDamage(10);

            GameObject hitParticles = Instantiate(HitFX, null);

            hitParticles.transform.position = hit.point;
            hitParticles.transform.forward  = hit.normal;

            Destroy(hitParticles, 2f);

            MRenderer.enabled = false;
            this.enabled      = false;
            RB.isKinematic = true;

            Destroy(gameObject,2f);
        }
    }
}
