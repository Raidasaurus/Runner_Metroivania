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
    PlayerController pc;
    PlayerManager pm;
    Rigidbody rb;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pm.keybind.dashKey)) StartDash();

        if (dashCdTimer > 0) dashCdTimer -= Time.deltaTime;
    }

    void StartDash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCD;

        pm.dashing = true;
        Vector3 dir = pm.orientation.forward * Input.GetAxisRaw("Vertical") + pm.orientation.right * Input.GetAxisRaw("Horizontal");
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
        rb.AddForce(delayedDashForce, ForceMode.Impulse);
    }

    void ResetDash()
    {
        pm.dashing = false;
        pm.cam.DoFov(60);
    }
}
