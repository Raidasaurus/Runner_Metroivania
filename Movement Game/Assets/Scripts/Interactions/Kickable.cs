using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kickable : MonoBehaviour
{
    public float debugVelocity;

    Rigidbody rb;
    public bool inMotion;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        debugVelocity = rb.velocity.magnitude;
        if (rb.velocity.magnitude > 0.1f) inMotion = true;
        else if (rb.velocity.magnitude <= 0.1f) inMotion = false;
    }
}
