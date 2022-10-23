using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject HitFX;
    public GameObject Shooter = null;
    Rigidbody RB;

    public float Duration = 2f;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, Duration);
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, RB.velocity.normalized, out RaycastHit hit, RB.velocity.magnitude * Time.fixedDeltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            IDamageable damagedAgent = hit.collider.gameObject.GetComponentInParent<IDamageable>();

            if (damagedAgent == null) damagedAgent = hit.collider.gameObject.GetComponent<IDamageable>();

            damagedAgent?.AddDamage(1, Shooter);

            GameObject hitParticles = Instantiate(HitFX, null);

            hitParticles.transform.position = hit.point;
            hitParticles.transform.forward  = hit.normal;

            Destroy(hitParticles, 2f);

            Destroy(gameObject);
        }
    }
}
