using NPA_Health_Components;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public Health PlayerHealth;
    
    public Hitstun stun;

    public LayerMask whatIsGround, whatIsPlayer;
    
    // Shooting
    public Transform bulletPoint;
    public GameObject ShotPrefab;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public bool RangedToogle;

    //States 
    public float sightRange, attackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awaked()
    {
        player = GameObject.Find("Sprite").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        PlayerHealth = player.GetComponent<Health>();
        //check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) Patrolling();
        if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
        if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
      

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            if (stun != null && stun.IsStunned) 
            {
                  Debug.Log("STUNNED!");
                  return; // skip attack
            }
              //Attack code
            if (RangedToogle == true)
            {
                Instantiate(ShotPrefab, bulletPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            }
            else
            {
                PlayerHealth.TakeDamage(10);
            }

                alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
