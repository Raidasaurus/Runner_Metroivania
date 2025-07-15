using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MovementScript
{
    [Header("Variables")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float dashCD;
    float dashCdTimer;
    Vector3 delayedDashForce;

    [Header("References")]
    PlayerManager pm;
    Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        pm.dash.performed += ctx => StartDash();
    }

    private void Update()
    {
        if (dashCdTimer > 0) dashCdTimer -= Time.deltaTime;
    }

    void StartDash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCD;

        pm.dashing = true;
        Vector3 dir = (pm.orientation.forward * Input.GetAxisRaw("Vertical") + pm.orientation.right * Input.GetAxisRaw("Horizontal")).normalized;
        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
            dir = pm.orientation.forward;

        Vector3 forceToApply = dir * dashForce;

        delayedDashForce = forceToApply;
        Invoke(nameof(DelayedDash), 0.025f);


        Invoke(nameof(ResetDash), dashDuration);
        pm.cam.DoFov(70);
    }

    void DelayedDash()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(delayedDashForce, ForceMode.VelocityChange);
    }

    void ResetDash()
    {
        pm.dashing = false;
        pm.cam.DoFov(60);
    }
}
