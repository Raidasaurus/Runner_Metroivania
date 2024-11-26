using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Variables")]
    public float health = 3f;
    public float stunDuration = 4f;
    public Transform nextPos;

    [Header("Detection Ranges")]
    public float resetRange;
    public float detectionRange;
    public float attackRange;

    [Header("State")]
    public EnemyState desiredState;
    [SerializeField] EnemyState state;
    public bool patrolling;
    [SerializeField] bool damageable = true;

    [Header("References")]
    PlayerController pc;
    NavMeshAgent agent;


    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(nextPos.position);
    }

    private void Update()
    {
        if (health <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }
        StateMachine();

        switch(state)
        {
            case EnemyState.idle:
                patrolling = false;
                break;            
            case EnemyState.patrol:
                if (!patrolling) agent.SetDestination(nextPos.position);
                agent.isStopped = false;
                patrolling = true;
                break;            
            case EnemyState.search:
                patrolling = false;
                break;            
            case EnemyState.chase:
                agent.SetDestination(pc.transform.position);
                patrolling = false;
                break;            
            case EnemyState.attack:
                patrolling = false;
                break;            
            case EnemyState.stunned:
                agent.isStopped = true;
                patrolling = false;
                break;
            default:
                break;
        }
    }

    void StateMachine()
    {
        if (state == EnemyState.stunned) return;

        if (Vector3.Distance(transform.position, pc.transform.position) <= attackRange)
        {
            state = EnemyState.attack;
        }
        else if (Vector3.Distance(transform.position, pc.transform.position) <= detectionRange)
        {
            state = EnemyState.chase;
        }
        else if (Vector3.Distance(transform.position, pc.transform.position) <= resetRange)
        {
            state = desiredState;
        }
        else
        {
            state = desiredState;
        }
    }

    public enum EnemyState
    {
        idle,
        patrol,
        search,
        chase,
        attack,
        stunned
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Kickable"))
        {
            if (collision.gameObject.GetComponent<Kickable>().inMotion)
            {
                if (damageable) StartCoroutine(TakeDamage());
            }
        }
    }



    IEnumerator TakeDamage()
    {
        damageable = false;
        state = EnemyState.stunned;
        health--;
        yield return new WaitForSeconds(stunDuration);
        state = desiredState;
        damageable = true;
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
