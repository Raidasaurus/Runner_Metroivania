using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("Variables")]
    public float charge;
    public float chargeMax;
    public float hitRadius;
    public LayerMask hitLayers;

    [Header("Knockback")]
    public float lightForce;
    public float heavyForce;
    public float kickUpForce;
    float force;

    [Header("References")]
    public Transform hitPos;
    PlayerManager pm;

    void Start()
    {
        pm = GetComponent<PlayerManager>();

        pm.attack.canceled += ctx => Attack();
    }

    void Update()
    {
        if (pm.attack.ReadValue<float>() > 0)
        {
            if (charge < chargeMax) charge += Time.deltaTime;
            else if (charge > chargeMax) charge = chargeMax;
        }
    }

    void Attack()
    {
        RaycastHit[] hits = CheckForHits();

        foreach (var hit in hits)
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && rb.GetComponent<Hitable>())
            {
                if (rb.GetComponent<Hitable>().knockback)
                {
                    Vector3 dir = (rb.position - transform.position);
                    dir = new Vector3(dir.x, 0, dir.z).normalized;
                    if (charge < chargeMax / 2) force = lightForce;
                    else if (charge > chargeMax * 0.9f) force = heavyForce;
                    rb.AddForce((dir.normalized * force) + (Vector3.up * kickUpForce), ForceMode.Impulse);
                }
                if (rb.GetComponent<Hitable>().damageable) rb.GetComponent<Hitable>().TakeDamage();
            }
        }
        charge = 0;
    }

    RaycastHit[] CheckForHits()
    {
        RaycastHit[] temp = Physics.SphereCastAll(hitPos.position, hitRadius, pm.orientation.forward, hitRadius, hitLayers);
        return temp;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(hitPos.position, hitRadius);
    }
}
