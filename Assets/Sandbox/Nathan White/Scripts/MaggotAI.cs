using UnityEngine;

public class MaggotAI : SimpleAi
{
    private Transform self;
    private SimpleAi FlyAi;
    public GameObject Bodyparts;

    void Start()
    {
        self = GetComponent<Transform>();
        FlyAi = GetComponent<SimpleAi>();
        //FlyAi.attackPointOffset = new Vector3(0, 1, 0);

        
    }

    public override void Update()
    {
        base.Update();
        if (!PlayerInSightRange)
        {
            Bodyparts.SetActive(true);
        }
        ;//Attack when player is in attack range
    }

    public override void ChasePlayer()
    {
        if (combatManager != null)
        {
            combatManager.CombatFuntion(); // Tells the Combat Manager to set combat to active
        }
        if (player != null)
        {
            agent.SetDestination(player.position); // Set the enemies destination to the player. (How it chases the player)
            Bodyparts.SetActive(false); // fake digging under ground.
        }
    }

    public override void AttackPlayer()
    {
        Bodyparts.SetActive(true);

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
}
