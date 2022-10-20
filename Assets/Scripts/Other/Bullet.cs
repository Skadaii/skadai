using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject HitFX;
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
        if(Physics.Raycast(transform.position, RB.velocity, out RaycastHit hit, RB.velocity.magnitude * Time.fixedDeltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            IDamageable damagedAgent = hit.collider.gameObject.GetComponentInParent<IDamageable>();

            if (damagedAgent == null) damagedAgent = hit.collider.gameObject.GetComponent<IDamageable>();

            damagedAgent?.AddDamage(1);

            GameObject hitParticles = Instantiate(HitFX, null);

            hitParticles.transform.position = hit.point;
            hitParticles.transform.forward  = hit.normal;

            Destroy(hitParticles, 2f);

            Destroy(gameObject);
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        IDamageable damagedAgent = collision.gameObject.GetComponentInParent<IDamageable>();
        if (damagedAgent == null)
            damagedAgent = collision.gameObject.GetComponent<IDamageable>();
        damagedAgent?.AddDamage(1);

        GameObject hitParticles = Instantiate(HitFX, null);

        hitParticles.transform.position = collision.contacts[0].
        hitParticles.transform.forward = -transform.forward;

        Destroy(hitParticles, 2f);

        Destroy(gameObject);
    }*/

}
