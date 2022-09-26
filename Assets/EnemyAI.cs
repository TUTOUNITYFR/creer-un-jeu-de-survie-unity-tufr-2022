using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Animator animator;

    [Header("Stats")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float chaseSpeed;

    [SerializeField]
    private float detectionRadius;

    [SerializeField]
    private float attackRadius;

    [SerializeField]
    private float attackDelay;

    [SerializeField]
    private float damageDealt;

    [SerializeField]
    private float rotationSpeed;

    [Header("Wandering parameters")]

    [SerializeField]
    private float wanderingWaitTimeMin;

    [SerializeField]
    private float wanderingWaitTimeMax;

    [SerializeField]
    private float wanderingDistanceMin;

    [SerializeField]
    private float wanderingDistanceMax;

    private bool hasDestination;
    private bool isAttacking;

    void Update()
    {
        if(Vector3.Distance(player.position, transform.position) < detectionRadius)
        {
            agent.speed = chaseSpeed;

            Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

            if(!isAttacking)
            {
                if (Vector3.Distance(player.position, transform.position) < attackRadius)
                {
                    StartCoroutine(attackPlayer());
                }
                else
                {
                    agent.SetDestination(player.position);
                }
            }
            
        }
        else
        {
            agent.speed = walkSpeed;

            if(agent.remainingDistance < 0.75f && !hasDestination)
            {
                StartCoroutine(GetNewDestination());
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    IEnumerator GetNewDestination()
    {
        hasDestination = true;
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));

        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin, wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1), 0f, Random.Range(-1f, 1f)).normalized;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        hasDestination = false;
    }

    IEnumerator attackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true;

        animator.SetTrigger("Attack");

        playerStats.TakeDamage(damageDealt);

        yield return new WaitForSeconds(attackDelay);
        agent.isStopped = false;
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
