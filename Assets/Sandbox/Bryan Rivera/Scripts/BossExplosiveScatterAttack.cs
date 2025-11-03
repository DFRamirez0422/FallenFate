using UnityEngine;

public class BossExplosiveScatterAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject explosiveProjectilePrefab;
    public Transform throwPoint;
    public float attackInterval = 10f;
    public int projectileCount = 5;
    public float scatterRadius = 5f;
    public float arcHeight = 5f;

    [Header("Target")]
    public Transform player;

    private float attackTimer;

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            ThrowExplosiveScatter();
            attackTimer = 0f;
        }
    }

    void ThrowExplosiveScatter()
    {
        if (!player || !explosiveProjectilePrefab || !throwPoint)
        {
            Debug.LogWarning("Missing references!");
            return;
        }

        for (int i = 0; i < projectileCount; i++)
        {
            // Random offset within scatter radius on XZ plane
            Vector3 randomOffset = new Vector3(
                Random.Range(-scatterRadius, scatterRadius),
                0f,
                Random.Range(-scatterRadius, scatterRadius)
            );

            Vector3 targetPosition = player.position + randomOffset;

            // Raise spawn position to avoid overlapping with the boss
            Vector3 spawnPosition = throwPoint.position + Vector3.up * 1.5f;

            // Debug line to visualize trajectory
            Debug.DrawLine(spawnPosition, targetPosition, Color.red, 2f);

            GameObject projectile = Instantiate(explosiveProjectilePrefab, spawnPosition, Quaternion.identity);

            // OPTIONAL: Force set layer to "Projectile" to ensure layer-based collision rules apply
            projectile.layer = LayerMask.NameToLayer("Projectile");

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            Collider projectileCollider = projectile.GetComponent<Collider>();
            Collider bossCollider = GetComponent<Collider>();

            if (rb)
            {
                Vector3 velocity = CalculateArcVelocity(spawnPosition, targetPosition, arcHeight);
                rb.linearVelocity = velocity;

                // Ignore collision with boss
                if (bossCollider && projectileCollider)
                {
                    Physics.IgnoreCollision(bossCollider, projectileCollider);
                }
            }
            else
            {
                Debug.LogWarning("Projectile prefab missing Rigidbody!");
            }
        }
    }

    Vector3 CalculateArcVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);
        float displacementY = end.y - start.y;

        float timeUp = Mathf.Sqrt(2 * height / gravity);
        float timeDown = Mathf.Sqrt(2 * Mathf.Max(0.01f, height - displacementY) / gravity);
        float totalTime = timeUp + timeDown;

        if (totalTime < 0.01f)
            totalTime = 0.5f;

        float velocityY = Mathf.Sqrt(2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + Vector3.up * velocityY;
    }
}
