using NPA_Health_Components;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;              // assign in Inspector or found in Awake
    public Health Health;                 // this enemy's own health

    public LayerMask whatIsGround, whatIsPlayer;

    // Shooting
    public Transform bulletPoint;
    public GameObject ShotPrefab;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks = 1f;
    bool alreadyAttacked;
    public bool RangedToogle;

    // States
    public float sightRange = 10f, attackRange = 2f;
    public bool PlayerInSightRange, PlayerInAttackRange;

    // ---- FIX: declare & use this instead of undefined 'PlayerHealth'
    private Health playerHealth;

    private void Awake() // FIX: Unity method name
    {
        if (player == null)
        {
            // Prefer a tag on your player object: tag it "Player"
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player != null && playerHealth == null)
            playerHealth = player.GetComponent<Health>(); // FIX: cache the component

        // check for sight and attack range
        PlayerInSightRange  = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) Patrolling();
        if (PlayerInSightRange && !PlayerInAttackRange)  ChasePlayer();
        if (PlayerInSightRange && PlayerInAttackRange)   AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walk point reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX,
                                transform.position.y,
                                transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint + Vector3.up, Vector3.down, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        if (player != null)
            transform.LookAt(player);

        if (alreadyAttacked) return;

        // Attack code
        if (RangedToogle)
        {
            if (ShotPrefab != null && bulletPoint != null)
                Instantiate(ShotPrefab, bulletPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up));
        }
        else
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(10); // FIX: use declared variable
        }

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
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
