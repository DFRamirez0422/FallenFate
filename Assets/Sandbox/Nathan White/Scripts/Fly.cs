using UnityEngine;
using UnityEngine.AI;

public class Fly : SimpleAi
{
    private Transform self;
    private SimpleAi FlyAi;
    public float ChargeDistance = 5f;

    void Start()
    {
        self = GetComponent<Transform>();
        FlyAi = GetComponent<SimpleAi>();
        FlyAi.attackPointOffset = new Vector3(0, 1, 0);
    }

    public override void Update()
    {
        base.Update();

        transform.position = transform.position + new Vector3(0, 1, 0);
    }

    public override void AttackPlayer()
    {
        if (player != null)
        {
            //Locks the enemies Y position so it doesn't look down
            if (!alreadyAttacked)
            {
                Vector3 lookPos = player.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);
            }
            //Make sure enemy doesn't move
            

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
                    Debug.Log("Attacking");
                }

                alreadyAttacked = true;

                Debug.Log($"{name} is attacking player.");

                agent.SetDestination(transform.position);
                Invoke(nameof(ResetAttack), timeBetweenAttacks); //This is what delays the attacks / timeBetweenAttacks is what adds a delay on the attacks.
            }
        }
    }

    public override void ResetAttack()
    {
        base.ResetAttack();
        agent.speed = 5;
    }
    public override void FlashAttackMelee()
    {
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        agent.SetDestination(transform.position + (transform.forward * ChargeDistance));
        agent.speed = 50;


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
}

