using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollision : MonoBehaviour
{
    public Grapple grapple;

    private void OnCollisionEnter(Collision collision)
    {
        grapple.GrappleCollide();
    }

    private void OnTriggerEnter(Collider other)
    {
        grapple.GrappleCollide();
    }
}
