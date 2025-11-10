using AAA_FallenFate.Scripts.PlayerScripts;
using NPA_Health_Components;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    [Header("References")]
    private Health PlayerHealth;
    protected CombatManager combatManager;
    public EnemyHitboxController hitboxController;
    private ParryBlock damageHandle;
    

    [Header("Layers")] 
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("Shooting")]
    public Transform attackPoint;
    public Vector3 attackPointOffset = new Vector3(0, 0, 0); // For now this is only for the fly cause its a buggy bitch. (I mean this in both ways) - Nathan
    public GameObject ShotPrefab;
    public bool RangedToogle = false;

    [Header("Patroling")] //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public Waypoint WaypointScript;
    public bool RandomMovementToogle = false;

    [Header("Attacking")]//Attacking 
    public float timeBetweenAttacks;
    protected bool alreadyAttacked;
    public GameObject MeleePrefab; // This is just a representation for now
    public GameObject MarkPrefab; //In enemies folder
    [SerializeField, Tooltip("Delay before attacking/after the mark appears")]
    protected float attackDelay = 1.0f;

    [Header("States")]//States 
    public float sightRange, attackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //Sets anything with the tag "Player" to player.
        agent = GetComponent<NavMeshAgent>(); // Gets its own navMeshAgent
        if (GameObject.FindGameObjectWithTag("Manager"))
        {
            combatManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CombatManager>(); //Automatically gets the CombatManager for Elenna 
        }
        hitboxController = GetComponent<EnemyHitboxController>();
    }

    public virtual void Update()
    {
        if (player != null)
        {
            PlayerHealth = player.GetComponent<Health>();//Automatically gets the players health.
        }

        //check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if (!PlayerInSightRange && !PlayerInAttackRange && RandomMovementToogle == false && !alreadyAttacked) Waypoints(); //When player is not in range follow waypoints;
        if (!PlayerInSightRange && !PlayerInAttackRange && RandomMovementToogle == true && !alreadyAttacked) Patrolling(); //When player is not in range enemy will move randomly;
        if (PlayerInSightRange && !PlayerInAttackRange && !alreadyAttacked) ChasePlayer();//When player is in sight range chase them down!
        if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();//Attack when player is in attack range

        // Debug to check if player is in sight and/or attack range - can be removed when not needed
        //Debug.Log($"Sight={PlayerInSightRange}, Attack={PlayerInAttackRange}");
    }

    //Script in case you want the enemy to move randomly instead of following the waypoints.
    //Not used Right Now!!!!
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

    //Makes the enemy follow waypoints
    private void Waypoints()
    {
        WaypointScript.Walking();
        agent.SetDestination(WaypointScript.waypoint[WaypointScript.currentWaypointIndex].position);
    }

    //Code for searching for a random Walkpoint
    //Not Used Right Now!!!
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    //Script for Chasing the player
    public virtual void ChasePlayer()
    {
        if (combatManager != null)
        {
            combatManager.CombatFuntion(); // Tells the Combat Manager to set combat to active
        }
        if ( player != null)
        {
            agent.SetDestination(player.position); // Set the enemies destination to the player. (How it chases the player)
        }
    }

    //Script for Attacking the player
    public virtual void AttackPlayer()
    {

        if (player != null)
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);
            
            //Locks the enemies Y position so it doesn't look down
            Vector3 lookPos = player.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            if (!alreadyAttacked)
            {
                //Attack code
                if (RangedToogle == true) // Checks if the range toogle is on
                {
                    Instantiate(ShotPrefab, attackPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up)); //Spawns the Shot
                }
                else // If range toggle is off, melee
                {
                    if (MarkPrefab != null)
                    {
                        Instantiate(MarkPrefab, attackPoint.position + attackPointOffset, Quaternion.LookRotation(transform.forward, Vector3.up)); //Spawns the mark 
                    }
                    Invoke(nameof(FlashAttackMelee), timeBetweenAttacks - attackDelay); // Delay so the attack comes out after the mark;
                }

                alreadyAttacked = true;
                
                Debug.Log($"{name} is attacking player.");
                
                Invoke(nameof(ResetAttack), timeBetweenAttacks); //This is what delays the attacks / timeBetweenAttacks is what adds a delay on the attacks.
            }
        }
    }

    
    public virtual void ResetAttack()
    {
        alreadyAttacked = false; //Sets alreadyAttacked to false allowing the enemy to attack again 
    }

    public virtual void FlashAttackMelee()
    { 
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;


            if (hitboxController != null)
            {
                // Pull the first available hitbox ID from the assigned data asset
                string attackID = hitboxController.GetHitboxIDByName("");
                if (!string.IsNullOrEmpty(attackID))
                {
                    hitboxController.ActivateHitbox(attackID);
                    Debug.Log($"{name} triggered FlashAttackMelee() → {attackID}");
                    
                }
                else
                {
                    Debug.LogWarning($"{name} has EnemyHitboxController but no valid hitboxes in its data asset!");
                }
            }
            else
            {
                Debug.LogWarning($"{name} has no EnemyHitboxController attached!");
            }
    }

    //Just for developers to see attack and sight range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
