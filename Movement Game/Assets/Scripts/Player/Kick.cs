using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [Header("Variables")]
    public float kickBackForce;
    public float kickUpForce;
    public float kickCD;
    public float kickSize;
    public LayerMask hitLayers;
    public bool objHeld = false;
    public GameObject currentObj;
    float kickCDTimer;
    float startYScale;

    [Header("References")]
    public Transform kickPoint;
    public Transform holdPoint;
    public bool debug;
    PlayerController pc;
    PlayerManager pm;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        pm = GetComponent<PlayerManager>();
        startYScale = holdPoint.transform.localScale.y;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (objHeld && currentObj != null) // Drop held obj
            {
                currentObj.GetComponent<Kickable>().Drop();
                objHeld = false;
            }
            else if (!objHeld && currentObj != null) // Throw held obj
            {
                currentObj.GetComponent<Kickable>().PickUp(holdPoint);
                objHeld = true;
            }
        }

        if (Input.GetKeyDown(pm.keybind.attackKey))
        {
            if (objHeld)
            {
                Throw();
            }
            else
                Attack();
        }
        if (kickCDTimer > 0) kickCDTimer -= Time.deltaTime;
    }

    void Throw()
    {
        Rigidbody rb = currentObj.GetComponent<Rigidbody>();
        currentObj.GetComponent<Kickable>().Drop();
        currentObj = null;
        objHeld = false;
        if (rb!= null)
        {
            rb.GetComponent<Kickable>().inMotion = true;
            Vector3 dir = rb.position - transform.position;
            rb.AddForce(dir.normalized * kickBackForce + Vector3.up * kickUpForce, ForceMode.Impulse);
        }
    }

    void Attack()
    {
        if (kickCDTimer > 0) return;
        else kickCDTimer = kickCD;

        RaycastHit[] hits = CheckForHits();

        foreach (var hit in hits)
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (rb.GetComponent<Kickable>()) rb.GetComponent<Kickable>().inMotion = true;

                Vector3 dir = rb.position - transform.position;
                rb.AddForce(dir.normalized * kickBackForce + Vector3.up * kickUpForce, ForceMode.Impulse);
            }
        }

    }

    RaycastHit[] CheckForHits()
    {
        RaycastHit[] temp = Physics.SphereCastAll(kickPoint.position, kickSize, pm.orientation.forward, kickSize, hitLayers);
        return temp;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;
        Gizmos.DrawSphere(kickPoint.position, kickSize);
    }
}
