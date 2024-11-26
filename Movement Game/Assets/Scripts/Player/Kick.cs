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
    float kickCDTimer;

    [Header("References")]
    public Transform kickPoint;
    public bool debug;
    PlayerController pc;
    PlayerManager pm;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        pm = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pm.keybind.attackKey)) Attack();

        if (kickCDTimer > 0) kickCDTimer -= Time.deltaTime;
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
