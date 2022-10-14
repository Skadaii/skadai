using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void MoveToward(Vector3 velocity)
    {
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }
}
