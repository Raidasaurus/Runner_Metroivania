using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MovementScript
{
    [Header("Variables")]
    public float climbSpeed;
    public float maxClimbTime;
    float climbTimer;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    float wallLookAngle;
    public Transform feetPos;

    RaycastHit frontWallHit;
    bool wallFront;

    [Header("References")]
    public LayerMask whatIsWall;
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
        // Add spacebar to wallcheck
        WallCheck();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.climbing) ClimbingMovement();
    }

    void WallCheck()
    {
        wallFront = Physics.SphereCast(feetPos.position, sphereCastRadius, pm.orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(pm.orientation.forward, -frontWallHit.normal);
        if (pc.grounded) climbTimer = maxClimbTime;
    }

    void StartClimbing()
    {
        pm.climbing = true;
    }

    void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    void StopClimbing()
    {
        pm.climbing = false;
    }

    void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && Input.GetKey(pm.keybind.jumpKey) && wallLookAngle < maxWallLookAngle)
        {
            if (!pm.climbing && climbTimer > 0)
                StartClimbing();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        else
        {
            if (pm.climbing) StopClimbing();
        }
    }
}
