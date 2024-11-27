using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementScript
{
    [Header("Debug")]
    public Vector3 DebugSpeed;
    public float DebugDesiredMoveSpeed;
    public MovementState state;

    float moveSpeed;

    [Header("Movement Multipliers")]
    public float lerpMultiplier;
    public float airLerpMultiplier;
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;

    [Header("Jumping")]
    public float jumpChargeTotal;
    public float jumpCharges;
    public float jumpForce;
    public float jumpCooldown;
    public float airMulti;

    [Header("Scale Values")]
    public float crouchYScale;
    float startYScale;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideCooldown;
    [SerializeField] float slideTimer;

    [Header("Ground Check")]
    public float groundDrag;
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Slope Handler")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Conditions")]
    public bool grounded;
    public bool readyToJump;
    public bool canSlide;
    public bool activeGrapple;

    [Header("References")]
    PlayerManager pm;
    [HideInInspector] public Rigidbody rb;

    float hInput;
    float vInput;
    Vector3 moveDir;


    public enum MovementState
    {
        walking,
        crouching,
        sliding,
        wallrun,
        dashing,
        climbing,
        freeze,
        air,
        idle
    }

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        canSlide = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);
        if (grounded) jumpCharges = jumpChargeTotal;

        GetInput();
        SpeedControl();
        StateHandler();

        if (pm.grappling)
            rb.drag = 0;
        else
            rb.drag = groundDrag;


        DebugSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        DebugDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (pm.sliding)
            SlidingMovement();

    }

    void GetInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        // Jumping
        if (Input.GetKeyDown(pm.keybind.jumpKey) && readyToJump && (jumpCharges > 0 || grounded) && !pm.wallrunning)
        {
            readyToJump = false;
            jumpCharges--;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch / Sliding start
        if (Input.GetKeyDown(pm.keybind.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            if (moveDir.magnitude > 0f && !pm.sliding && grounded && canSlide)
            {
                StartSlide();
            }
        }

        // Crouch / Sliding end
        if (Input.GetKeyUp(pm.keybind.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            if (pm.sliding)
            {
                StopSlide();
            }
        }

        // Inventory
        if (Input.GetKeyDown(pm.keybind.inventoryKey))
        {
            pm.cam.lockCursor = !pm.cam.lockCursor;
            pm.aniUI.SetTrigger("Toggle");
        }

    }



    void StateHandler()
    {
        if (pm.freeze)
        {
            state = MovementState.freeze;
            rb.velocity *= 0.98f;
        }
        else if (pm.climbing) // Climbing
        {
            state = MovementState.climbing;
            desiredMoveSpeed = pm.wallRunSpeed;
        }
        else if (pm.dashing) // Dashing
        {
            state = MovementState.dashing;
            desiredMoveSpeed = pm.dashSpeed;
            activeGrapple = false;
            pm.freeze = false;
            pm.grappling = false;
        }
        else if (pm.wallrunning) // Wallrunning
        {
            state = MovementState.wallrun;
            desiredMoveSpeed = pm.wallRunSpeed;
            StopAllCoroutines();
        }
        else if (pm.sliding) // Sliding
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = pm.slideSpeed + 15f;
            else
                desiredMoveSpeed = pm.slideSpeed;

        }
        else if (Input.GetKey(pm.keybind.crouchKey) && !pm.sliding && grounded) // Crouching
        {
            state = MovementState.crouching;
            desiredMoveSpeed = pm.crouchSpeed;
        }
        else if (grounded && !pm.sliding && moveDir.magnitude > 0) // Walking
        {

            state = MovementState.walking;
            desiredMoveSpeed = pm.walkSpeed;
            StopAllCoroutines();
            //ani.CrossFade(Moving, 0, 0);
        }
        else if (!grounded) // Jumping
        {
            state = MovementState.air;
            desiredMoveSpeed = lastDesiredMoveSpeed;
            StopAllCoroutines();
        }
        else // Idle
        {
            state = MovementState.idle;
            desiredMoveSpeed = pm.airSpeed;
        }
                

        if ((desiredMoveSpeed < moveSpeed) && (moveSpeed != 0f) && desiredMoveSpeed != lastDesiredMoveSpeed && !activeGrapple)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMovement());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    void MovePlayer()
    {
        moveDir = pm.orientation.forward * vInput + pm.orientation.right * (hInput * pm.strafeFactor);

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDir(moveDir) * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            rb.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMulti, ForceMode.Force);
        }

        if (!pm.wallrunning) rb.useGravity = !OnSlope();

        if (hInput > 0 && grounded && pm.cam.lockCursor) pm.cam.DoTilt(-2f);
        else if (hInput < 0 && grounded && pm.cam.lockCursor) pm.cam.DoTilt(2f);
        else pm.cam.DoTilt(0f);
    }

    void SpeedControl()
    {
        if (activeGrapple) return;

        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.8f))
        {
            if (slopeHit.transform.CompareTag("Player") || slopeHit.transform.CompareTag("Item")) return false;

            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, slopeHit.normal).normalized;
    }

    IEnumerator SmoothLerpMovement()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;
        float multi;

        while (time < difference)
        {
            if (!grounded)
                multi = airLerpMultiplier;
            else
                multi = lerpMultiplier;

            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, (time / difference) * multi);
            
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;
            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
        
    }

    void StartSlide()
    {
        pm.sliding = true;
        canSlide = false;
        slideTimer = maxSlideTime;
        pm.cam.DoFov(70);
    }

    void StopSlide()
    {
        pm.sliding = false;
        pm.cam.DoFov(60);
        Invoke("StartSlideCooldown", slideCooldown);
    }

    void StartSlideCooldown()
    {
        canSlide = true;
    }

    void SlidingMovement()
    {
        Vector3 inputDir = pm.orientation.forward * vInput + pm.orientation.right * hInput;

        if (!OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
            if (!grounded)
                slideTimer = maxSlideTime;
        }
        else
        {
            rb.AddForce(GetSlopeMoveDir(inputDir) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
            
        }
    }

}
