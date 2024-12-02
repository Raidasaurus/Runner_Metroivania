using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kickable : MonoBehaviour
{

    public Transform parent;

    Rigidbody rb;
    public bool inMotion;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    public void PickUp(Transform parentIn)
    {
        transform.SetParent(parentIn);
        transform.position = parentIn.position;
        rb.isKinematic = true;
    }

    public void Drop()
    {
        transform.SetParent(parent);
        rb.isKinematic = false;
    }

    void LateUpdate()
    {
        if (rb.velocity.magnitude > 0.1f) inMotion = true;
        else if (rb.velocity.magnitude <= 0.1f) inMotion = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<Kick>().objHeld)
                other.GetComponent<Kick>().currentObj = this.gameObject;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<Kick>().objHeld)
                other.GetComponent<Kick>().currentObj = null;
        }
    }
}
