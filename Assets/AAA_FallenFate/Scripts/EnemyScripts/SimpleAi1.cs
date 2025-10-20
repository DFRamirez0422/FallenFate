using AAA_FallenFate.Scripts.PlayerScripts;
using NPA_Health_Components;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public Health PlayerHealth;
    public CombatManager combatManager;

    public LayerMask whatIsGround, whatIsPlayer;
    
    // Shooting
    public Transform attackPoint;
    public GameObject ShotPrefab;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public Waypoint WaypointScript;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public bool RangedToogle = false;
    public GameObject MeleePrefab; // This is just a representation for now
    public GameObject MarkPrefab; //In enemies folder

    //States 
    public float sightRange, attackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;
    
    //erik
    private Hitstun hitstun;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //Sets anything with the tag "Player" to player.
        agent = GetComponent<NavMeshAgent>(); // Gets its own navMeshAgent
        combatManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CombatManager>(); //Automaticlly gets the CombatManager for Elenna
        ////ERIks hitstun
        hitstun = GetComponent<Hitstun>();
    }

    private void Update()
    {
        //If Stunned, Skip AI behavior
        if (hitstun != null && hitstun.isStunned)
        {
            agent.SetDestination(transform.position);
            return;
        }
        if (player != null)
        {
            PlayerHealth = player.GetComponent<Health>();//Automaticlly gets the players health.
        }

        //check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) Waypoints(); //When player is not in range follow waypoints;
        if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();//When player is in sight range chase them down!
        if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();//Attack when player is in attack range

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

    //Script for Chasing the playe
    private void ChasePlayer()
    {
        combatManager.CombatFuntion(); // Tells the Combat Manager to set combat to active
        if ( player != null)
        {
            agent.SetDestination(player.position); // Set the enimes destination to the player. (How it chases the player)
        }
    }

    //Script for Attacking the player
    private void AttackPlayer()
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
                    Instantiate(MarkPrefab, attackPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up)); //Spawns the mark
                    Invoke(nameof(FlashAttackMelee), timeBetweenAttacks - 1.5f); //Deley so the attack comes out after the mark;
                }

                alreadyAttacked = true;
                
                

                Invoke(nameof(ResetAttack), timeBetweenAttacks); //This is what delays the attacks //timeBetweenAttacks is what adds a delay on the attacks.
            }
        }
    }

    
    private void ResetAttack()
    {
        alreadyAttacked = false; //Sets alreadyAttacked to false allowing the enemy to attack again 
    }

    private void FlashAttackMelee()
    {
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        if (PlayerInSightRange && PlayerInAttackRange) //An extra check for when the attack comes out to make sure your in range
        {
            Instantiate(MeleePrefab, lookPos, Quaternion.LookRotation(transform.forward, Vector3.up)); //Spawns the Melee placeholder. Replace with a cool animation once we have them.
            player.GetComponent<ParryBlock>()?.TakeIncomingDamage(10, this.gameObject);
        }
        else
        {
            Instantiate(MeleePrefab, attackPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up)); //Spawns the Melee placeholder. Replace with a cool animation once we have them.
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
