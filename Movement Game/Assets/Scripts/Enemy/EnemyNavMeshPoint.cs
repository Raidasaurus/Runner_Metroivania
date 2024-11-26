using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshPoint : MonoBehaviour
{
    public Transform nextPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<NavMeshAgent>().SetDestination(nextPos.position);
            Debug.Log("Next Pos");
        }
    }
}
