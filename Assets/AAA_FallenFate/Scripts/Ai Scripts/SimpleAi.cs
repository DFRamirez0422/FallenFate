using NPA_Health_Components;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public Health PlayerHealth;

    public LayerMask whatIsGround, whatIsPlayer;
    
    // Shooting
    public Transform bulletPoint;
    public GameObject ShotPrefab;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public Waypoint WaypointScript;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject fist;
    public bool RangedToogle = false;
    private float fistTimer = 1;

    //States 
    public float sightRange, attackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player != null)
        {
            PlayerHealth = player.GetComponent<Health>();
        }
        //check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) Waypoints();
        if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
        if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();

        if (fist == true)
        {
            fistTimer = fistTimer - Time.deltaTime;
        }
        if (fistTimer <= 0)
        {
            fist.SetActive(false);
            fistTimer = 1;
        }
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

    private void Waypoints()
    {
            WaypointScript.Walking();
        agent.SetDestination(WaypointScript.waypoint[WaypointScript.currentWaypointIndex].position);
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
        if ( player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        if (player != null)
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                //Attack code
                if (RangedToogle == true)
                {
                    Instantiate(ShotPrefab, bulletPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                }
                else
                {
                    fist.SetActive(true);
                    PlayerHealth.TakeDamage(10);
                }

                alreadyAttacked = true;
                
                

                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
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
