using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Ranges")]
    public float resetRange;
    public float detectionRange;
    public float attackRange;

    private void Update()
    {
        
    }

    void StateMachine()
    {

    }

    public enum EnemyState
    {
        idle,
        patrol,
        search,
        attack
    }


    private void OnDrawGizmosSelected()
    {
        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);        
        
        // Detection Range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);        
        
        // Reset Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, resetRange);

    }
}
