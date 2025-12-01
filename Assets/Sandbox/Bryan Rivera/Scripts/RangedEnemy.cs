using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("References")]
    public GameObject potionPrefab;     // The projectile to throw
    public Transform firePoint;         // Where the potion spawns from
    public Transform player;            // Target to throw at

    [Header("Attack Settings")]
    public float throwInterval = 3f;    // Time between throws
    public float flightTime = 1.5f;     // Time it takes for the potion to reach the target
    public float attackRange = 15f;     // Only throw if player is within this distance

    private float timer = 0f;

    public float range = 10f;   // Range of the enemy to throw the potion

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange) return;

        // Face the player
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0f;
        transform.forward = lookDirection;

        timer += Time.deltaTime;
        if (timer >= throwInterval)
        {
            if (IsPlayerInRange())
            {
                ThrowPotionAtPlayer();
            }
            timer = 0f;
        }
    }

    bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    void ThrowPotionAtPlayer()
    {
        GameObject potion = Instantiate(potionPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = potion.GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.useGravity = true;

        Vector3 startPos = firePoint.position;
        Vector3 targetPos = player.position;

        // Calculate initial velocity needed to reach target in flightTime seconds
        Vector3 velocity = CalculateArcVelocity(startPos, targetPos, flightTime);
        rb.linearVelocity = velocity;
    }

    Vector3 CalculateArcVelocity(Vector3 start, Vector3 end, float time)
    {
        Vector3 distance = end - start;
        Vector3 distanceXZ = new Vector3(distance.x, 0f, distance.z);

        float verticalDistance = distance.y;
        float horizontalDistance = distanceXZ.magnitude;

        float verticalVelocity = (verticalDistance + 0.5f * Mathf.Abs(Physics.gravity.y) * time * time) / time;
        Vector3 horizontalVelocity = distanceXZ / time;

        Vector3 result = horizontalVelocity + Vector3.up * verticalVelocity;
        return result;
    }
}
