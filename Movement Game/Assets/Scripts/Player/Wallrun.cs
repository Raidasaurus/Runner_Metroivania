using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrun : MovementScript
{
    [Header("Variables")]
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    float wallRunTimer;

    float hInput, vInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    bool wallLeft;
    bool wallRight;

    [Header("Exit Wall")]
    public float exitWallTime;
    bool exitingWall;
    float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float counterGravity;

    [Header("References")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
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
        CheckForWall();
        StateMachine();
        if (!pm.wallrunning) pm.cam.DoTilt(0f);
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunMovement();
    }

    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, pm.orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -pm.orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    void StateMachine()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if ((wallLeft || wallRight) && vInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(pm.keybind.jumpKey))
                WallJump();

            pc.jumpCharges = pc.jumpChargeTotal;
        }
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    void StartWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (wallLeft) pm.cam.DoTilt(-5f);
        if (wallRight) pm.cam.DoTilt(5f);
    }

    void WallRunMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((pm.orientation.forward - wallForward).magnitude > (pm.orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if(!(wallLeft && hInput > 0) && !(wallRight && hInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        if (useGravity)
            rb.AddForce(transform.up * counterGravity, ForceMode.Force);
    }

    void StopWallRun()
    {
        pm.wallrunning = false;
        pm.cam.DoTilt(0f);
    }

    void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;


        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        pm.cam.DoTilt(0f);
    }
}
