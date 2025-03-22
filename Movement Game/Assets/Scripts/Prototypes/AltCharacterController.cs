using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltCharacterController : MonoBehaviour
{
    [Header("Player")]
    public float playerHeight;
    public float playerSpeed;
    public float jumpForce;

    [Header("Spring")]
    public float springStrength;
    public float springDamper;
    public float springHeight;
    public LayerMask whatIsGround;
    bool grounded;
    RaycastHit rayHit;

    [Header("Reference")]
    public Rigidbody rb;

    float hInput;
    float vInput;
    Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, out rayHit, playerHeight, whatIsGround);
        GetInput();
    }

    private void FixedUpdate()
    {
        if (grounded) ApplySpringForce();
        MovePlayer();
    }

    void GetInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    void MovePlayer()
    {
        moveDir = transform.forward * vInput + transform.right * hInput;

        rb.AddForce(moveDir * playerSpeed, ForceMode.Force);
        if (rb.velocity.magnitude > playerSpeed)
            rb.velocity = rb.velocity.normalized * playerSpeed;
    }

    void Jump()
    {
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ApplySpringForce()
    {
        Vector3 vel = rb.velocity;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null) otherVel = hitBody.velocity;

        float rayDirVel = Vector3.Dot(Vector3.down, vel);
        float otherRayDirVel = Vector3.Dot(Vector3.down, otherVel);

        float relVel = rayDirVel - otherRayDirVel;

        float x = rayHit.distance - (playerHeight + springHeight);

        float springForce = (x * springStrength) - (relVel * springDamper);
        rb.AddForce(Vector3.down * springForce);
        if (hitBody != null) hitBody.AddForceAtPosition(Vector3.down * -springForce, rayHit.point);
    }

}
