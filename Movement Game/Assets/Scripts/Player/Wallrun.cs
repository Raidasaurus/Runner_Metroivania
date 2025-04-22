using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrun : MovementScript
{
    [Header("Variables")]
    public float o2Scalar;
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
    [SerializeField] bool aboveGround;
    [SerializeField] bool wallLeft;
    [SerializeField] bool wallRight;

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
    public PlayerManager pm;
    Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        pm.jump.performed += ctx => WallJump();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, pm.orientation.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, -pm.orientation.right * wallCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.down * minJumpHeight);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    void StateMachine()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        aboveGround = AboveGround();
        if ((wallLeft || wallRight) && vInput > 0 && aboveGround && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
                pm.o2 -= Time.deltaTime * o2Scalar;
            }

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
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
        if ((wallLeft || wallRight) && vInput > 0 && aboveGround && !exitingWall)
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
}
