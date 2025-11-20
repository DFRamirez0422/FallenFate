using UnityEngine;
using UnityEngine.AI;

// ============================================
// BEETLE AI - Movement, Patrol, Chase, Attack
// ============================================
public class AcidSprayBeetleAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator targetAnimator;


    [Header("Attack Settings")]
    public Transform attackPoint;
    public GameObject projectilePrefab;
    [Range(5f, 30f)] public float sightRange = 20f;
    [Range(1f, 10f)] public float stoppingDistance = 5f;
    [Range(1f, 10f)] public float attackCooldown = 5f;
    [Range(0.5f, 5f)] public float attackDuration = 2f;
    [Range(1f, 10f)] public float rechargeDuration = 3f;

    [Header("Projectile Settings")]
    [Range(5f, 30f)] public float projectileSpeed = 15f;
    [Range(1f, 10f)] public float projectileLifetime = 3f;
    [Range(3, 15)] public int projectileCount = 5;
    [Range(10f, 90f)] public float coneAngle = 30f;

    [Header("Patrol Settings")]
    [Range(5f, 30f)] public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    private enum BeetleState { Patrol, Chase, Attacking, Recharging }
    private BeetleState currentState = BeetleState.Patrol;
    private float stateTimer;
    private bool playerInSightRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            Debug.LogWarning("Player not found! Make sure player has 'Player' tag.");

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    private void Update()
    {
        if (player == null) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        switch (currentState)
        {
            case BeetleState.Patrol:
                Patrol();
                if (playerInSightRange)
                {
                    ChangeState(BeetleState.Chase);
                }
                break;

            case BeetleState.Chase:
                ChasePlayer();
                stateTimer += Time.deltaTime;

                if (!playerInSightRange)
                {
                    ChangeState(BeetleState.Patrol);
                }
                else if (stateTimer >= attackCooldown)
                {
                    ChangeState(BeetleState.Attacking);
                }
                break;

            case BeetleState.Attacking:
                AttackPlayer();
                stateTimer += Time.deltaTime;

                if (stateTimer >= attackDuration)
                {
                    ShootAcidCone();
                    ChangeState(BeetleState.Recharging);
                }
                break;

            case BeetleState.Recharging:
                RechargeWhileMoving();
                stateTimer += Time.deltaTime;

                if (stateTimer >= rechargeDuration)
                {
                    if (playerInSightRange)
                    {
                        ChangeState(BeetleState.Chase);
                    }
                    else
                    {
                        ChangeState(BeetleState.Patrol);
                    }
                }
                break;
        }
    }

    private void Patrol()
    {
        if (agent == null) return;

        agent.isStopped = false;

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null && agent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);

                if (agent.velocity.sqrMagnitude > 0.1f)
                {
                    Vector3 direction = agent.velocity.normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                agent.isStopped = true;

                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = transform.position.y;
                transform.LookAt(lookDirection);

                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
               // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void AttackPlayer()
    {
        if (player != null && agent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
            }

            Vector3 lookDirection = (player.position);
            lookDirection.y = transform.position.y;
            transform.LookAt(lookDirection);
        }
    }

    private void RechargeWhileMoving()
    {
        if (player != null && agent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);

                if (agent.velocity.sqrMagnitude > 0.1f)
                {
                    Vector3 direction = agent.velocity.normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                agent.isStopped = true;

                Vector3 lookDirection = (player.position - transform.position).normalized;
                transform.LookAt(lookDirection);
                lookDirection.y = transform.position.y;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
               // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void ShootAcidCone()
    {
        if (projectilePrefab == null || attackPoint == null || player == null)
        {
            Debug.LogWarning("Missing references for acid spray attack!");
            return;
        }
        targetAnimator.SetTrigger("Spit");

       


        Vector3 directionToPlayer = (player.position - attackPoint.position).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = -coneAngle / 2 + (coneAngle / (projectileCount - 1)) * i;
            Vector3 shootDirection = Quaternion.Euler(0, angle, 0) * directionToPlayer;

            Quaternion projectileRotation = Quaternion.LookRotation(shootDirection);

            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, projectileRotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = shootDirection * projectileSpeed;
            }

            Destroy(projectile, projectileLifetime);
        }
    }

    private void ChangeState(BeetleState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (player != null && attackPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = (player.position - transform.position).normalized;
            Gizmos.DrawRay(attackPoint.position, direction * 5f);
        } }


        


}
