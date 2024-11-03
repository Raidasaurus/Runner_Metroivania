using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MovementScript
{
    [Header("Grappling")]
    public LineRenderer lr;
    public Transform cameraPoint;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootY;
    public float grappleCD;
    float grappleCDTimer;
    Vector3 grapplePoint;
    RaycastHit grappleHit;

    [Header("Grapple Animation")]
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    Spring spring;
    public AnimationCurve affectCurve;

    [Header("References")]
    public bool debug;
    public GameObject testPrefab;
    PlayerController pc;
    PlayerManager pm;
    Rigidbody rb;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pm.keybind.grappleKey))
        {
            ResetGrappleAnimation();
            StartGrapple();
        }

        if (Input.GetKeyDown(pm.keybind.jumpKey)) StopGrapple();
        if (grappleCDTimer > 0) grappleCDTimer -= Time.deltaTime;
    }

    void JumpToPosition(Vector3 targetPos, float t)
    {
        pc.activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPos, t);
        Invoke(nameof(SetVelocity), 0.1f);

        //Invoke(nameof(ResetRestrictions), 3f);
    }

    Vector3 velocityToSet;

    void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    void ResetRestrictions()
    {
        pc.activeGrapple = false;
    }

    public void GrappleCollide()
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        if (!pm.grappling)
        {
            ResetGrappleAnimation();
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.update(Time.deltaTime);

        var up = Quaternion.LookRotation(grapplePoint - gunTip.position).normalized * Vector3.up;

        lr.SetPosition(0, gunTip.position);

        for (int i = 1; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTip.position, grapplePoint, delta) + offset);
        }
    }
     void ResetGrappleAnimation()
    {
        spring.Reset();
        if (lr.positionCount > 0)
            lr.positionCount = 0;
    }

    bool enableMovementOnNextTouch;

    void StartGrapple()
    {
        if (grappleCDTimer > 0) return;

        pm.grappling = true;
        pm.freeze = true;



        if (Physics.Raycast(cameraPoint.position, cameraPoint.forward, out grappleHit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = grappleHit.point;

            Invoke(nameof(GrappleMovement), grappleDelayTime);
            //Debug.Log("Hit Grapple Point: + " + grappleHit.collider.name);
        }
        else
        {
            grapplePoint = cameraPoint.position + cameraPoint.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
            //Instantiate(testPrefab, grapplePoint, Quaternion.identity);
            //Debug.Log("Missed Grapple Point: + " + grappleHit);
        }
        if (debug) Instantiate(testPrefab, grapplePoint, Quaternion.identity);
        lr.enabled = true;
    }

    void GrappleMovement()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelYPos + overshootY;

        if (grapplePointRelYPos < 0) highestPointOnArc = overshootY;
        JumpToPosition(grapplePoint, highestPointOnArc);

    }

    void StopGrapple()
    {
        pm.freeze = false;
        pm.grappling = false;

        grappleCDTimer = grappleCD;

        lr.enabled = false;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float g = Physics.gravity.y;
        float dy = endPoint.y - startPoint.y;
        Vector3 dxz = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 vy = Vector3.up * MathF.Sqrt(-2 * g * trajectoryHeight);
        Vector3 vxz = dxz / (MathF.Sqrt(-2 * trajectoryHeight / g) + MathF.Sqrt(2 * (dy - trajectoryHeight) / g));

        return vy + vxz;
    }
}

