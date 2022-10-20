using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAgent : Agent, IDamageable
{
    [SerializeField]
    float ShootFrequency = 1f;

    float NextShootDate = 0f;

    GameObject Target = null;

    Vector3 DeltaVel = Vector3.zero;

    protected override void OnDeath()
    {
        gameObject.SetActive(false);
    }

    void ShootForward()
    {
        // instantiate bullet
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate<GameObject>(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);
        }
    }

    void Start()
    {
        GunTransform = transform.Find("Body/Gun");
        if (GunTransform == null)
            Debug.Log("could not find gun transform");

        CurrentHP = MaxHP;
    }

    void Update()
    {
        if (Target)
        {
            Vector3 desiredForward = Vector3.Normalize(Target.transform.position - transform.position);

            transform.forward = Vector3.SmoothDamp(transform.forward, desiredForward, ref DeltaVel, 0.1f);

            // look at target position

            if (Vector3.SqrMagnitude(transform.forward - desiredForward) < 0.01f && Time.time >= NextShootDate)
            {
                NextShootDate = Time.time + ShootFrequency;
                ShootForward();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Target == null && other.gameObject.layer == LayerMask.NameToLayer("Allies"))
        {
            Target = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Target != null && other.gameObject == Target)
        {
            Target = null;
        }
    }
}
