using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MovementScript
{
    [Header("Debug")]
    public float DebugSpeed;
    public float DebugDesiredMoveSpeed;
    [SerializeField] MovementState state;

    [Header("Movement")]
    public float lerpMultiplier;
    public float accelRate;
    public float decelRate;
    public float stopLerpSpeed = 10f;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCD;
    public float jumpCheck;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideCooldown;
    [SerializeField] float slideTimer;

    [Header("Spring")]
    public LayerMask whatIsGround;
    public float dampFactor = 1;
    public float dampFrequency = 15;
    [SerializeField] float hoverHeight;
    public float hoverHeightStanding = 1f;
    public float hoverHeightCrouching = 0.5f;
    public float hoverHeightCrouchMoving = 0.5f;
    public float maxDistance = 2;
    public float castRadius = .5f;
    RaycastHit hit = new RaycastHit();




    [Header("References")]
    PlayerManager pm;
    [HideInInspector] public Rigidbody rb;

    float moveSpeed;
    float desiredMoveSpeed;
    float hInput;
    float vInput;
    Vector3 moveDir;

    [Header("Conditions")]
    public bool grounded = true;
    public bool canJump = true;
    public bool canSlide = true;
    bool canCheckForGround = true;

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

        pm.jump.performed += ctx => Jump();
        pm.crouch.performed += ctx => Crouch();
        pm.crouch.canceled += ctx => Stand();
        pm.openInventory.performed += ctx => OpenInventory();
    }


    private void Update()
    {
        if (!grounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, hoverHeight - jumpCheck, whatIsGround))
            {
                Invoke(nameof(ResetJump), jumpCD);
                if (canCheckForGround) grounded = true;
            }
        }
        
        GetInput();
        StateHandler();
        SpeedControl();

        DebugSpeed = rb.velocity.magnitude;
        DebugDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void FixedUpdate()
    {
        if (grounded) ApplyHoverForce();
        MovePlayer();
        if (pm.sliding)
            SlidingMovement();
    }

    void GetInput()
    {
        Vector2 input = pm.move.ReadValue<Vector2>();
        moveDir = (pm.orientation.forward * input.y + pm.orientation.right * input.x).normalized;

        hoverHeight = (pm.crouching || pm.sliding) ? (moveDir.magnitude > 0 ? hoverHeightCrouchMoving : hoverHeightCrouching) : hoverHeightStanding;

        
        // update this 
        /*
        if (Input.GetKeyDown(pm.keybind.inventoryKey) && grounded)
        {
            pm.cam.lockCursor = !pm.cam.lockCursor;
            pm.aniUI.SetTrigger("Toggle");
        }
        */
    }

    void OpenInventory()
    {
        pm.cam.lockCursor = !pm.cam.lockCursor;
        pm.aniUI.SetTrigger("Toggle");
    }

    void Crouch()
    {
        pm.crouching = true;
        if (moveDir.magnitude > 0f && !pm.sliding && grounded)
        {
            StartSlide();
        }
    }

    void Stand()
    {
        pm.crouching = false;
        if (pm.sliding)
        {
            StopSlide();
        }
    }
    
    void StateHandler()
    {
        if (pm.wallrunning) // Wall Running
        {
            state = MovementState.wallrun;
            desiredMoveSpeed = pm.wallRunSpeed;
        }
        else if (pm.dashing) // Dash
        {
            state = MovementState.dashing;
            desiredMoveSpeed = pm.dashSpeed;
        }
        else if (pm.sliding) // Sliding
        {
            state = MovementState.sliding;
            desiredMoveSpeed = pm.slideSpeed;
        }
        else if (pm.crouching && !pm.sliding) // Crouching
        {
            state = MovementState.crouching;
            desiredMoveSpeed = pm.crouchSpeed;
        }
        else if (moveDir.magnitude > 0f) // Walking
        {
            state = MovementState.walking;
            desiredMoveSpeed = pm.walkSpeed;
        }
        else
        {
            state = MovementState.idle;
            desiredMoveSpeed = 0f;
        }

    }

    void SpeedControl()
    {
        if (pm.dashing) return;


        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > desiredMoveSpeed && moveDir.magnitude > 0.1f)
        {
            Vector3 limitedVel = flatVel.normalized * desiredMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void MovePlayer()
    { 

        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 targetVelocity = moveDir * desiredMoveSpeed;

        float acceleration = (moveDir != Vector3.zero) ? accelRate : decelRate;

        // **Use physics-based acceleration**
        Vector3 newVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply force for acceleration
        Vector3 force = (newVelocity - currentVelocity) * rb.mass / Time.fixedDeltaTime;
        rb.AddForce(force, ForceMode.Force);

        // **Snappy stopping using Lerp**
        if (moveDir == Vector3.zero && currentVelocity.magnitude > 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * stopLerpSpeed);
        }
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
        canSlide = false;
    }

    void SlidingMovement()
    {
        Vector3 forceToApply = moveDir * slideForce;
        rb.AddForce(forceToApply, ForceMode.VelocityChange);

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    void Jump()
    {
        if (!canJump || pm.wallrunning) return;

        canJump = false;
        grounded = false;
        canCheckForGround = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetGroundCheck), 0.1f);
    }

    void ResetJump()
    {
        canJump = true;
    }

    void ResetGroundCheck()
    {
        canCheckForGround = true;
    }

    void ApplyHoverForce()
    {
        if (Physics.SphereCast(transform.position, castRadius, Vector3.down, out hit, hoverHeight, whatIsGround))
        {
            Vector3 rayDirection = Vector3.down;
            float springDelta = GetSpringDelta(hit);
            float springStrength = SpringStrength(rb.mass, dampFrequency);
            float dampStrength = DampStrength(dampFactor, rb.mass, dampFrequency);
            float springSpeed = GetRelativeSpeedAlongDirection(rb, hit.rigidbody, rayDirection);
            Vector3 springForce = GetSpringForce(
                springDelta,
                springSpeed,
                springStrength,
                dampStrength,
                rayDirection);
            springForce -= Physics.gravity;
            rb.AddForce(springForce);
            if (hit.rigidbody) hit.rigidbody.AddForceAtPosition(-springForce, hit.point);
        }
    }


    float GetSpringDelta(RaycastHit hit)
    {
        return hit.distance - (hoverHeight - castRadius);
    }

    static float GetRelativeSpeedAlongDirection(
        Rigidbody targetBody,
        Rigidbody frameBody,
        Vector3 direction)
    {
        Vector3 velocity = targetBody.velocity;
        Vector3 hitBodyVelocity = frameBody ? frameBody.velocity : default;
        float rayDirectionSpeed = Vector3.Dot(direction, velocity);
        float hitBodyRayDirectionSpeed = Vector3.Dot(direction, hitBodyVelocity);
        return rayDirectionSpeed - hitBodyRayDirectionSpeed;
    }

    static float SpringStrength(float mass, float frequency)
    {
        return frequency * frequency * mass;
    }

    static float DampStrength(float dampFactor, float mass, float frequency)
    {
        float criticalDampStrength = 2 * mass * frequency;
        return dampFactor * criticalDampStrength;
    }

    static Vector3 GetSpringForce(
        float springDelta,
        float springSpeed,
        float springStrength,
        float dampStrength,
        Vector3 direction)
    {
        float tension = springDelta * springStrength;
        float damp = springSpeed * dampStrength;
        float forceMagnitude = tension - damp;
        Vector3 force = direction * forceMagnitude;
        return force;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (hoverHeight - jumpCheck));
        Gizmos.DrawSphere(transform.position + Vector3.down * (hoverHeight), castRadius);
    }
}
