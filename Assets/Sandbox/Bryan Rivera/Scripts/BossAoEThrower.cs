using UnityEngine;
using System.Collections;
using NPA_PlayerPrefab.Scripts;

public class BossAoEThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject puddleWarningPrefab;
    public GameObject explosiveWarningPrefab;
    public GameObject bigExplosiveWarningPrefab;

    public GameObject puddleProjectilePrefab;
    public GameObject explosiveProjectilePrefab;
    public GameObject bigExplosiveProjectilePrefab;

    public Transform throwPoint;
    public Transform player;

    [Header("Settings")]
    public float throwInterval = 3f;
    public float arcHeight = 5f;
    public float projectileSpeed = 12f;
    public float warningLifetime = 2.5f;

    private float throwTimer = 0f;
    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocity;

    void Start()
    {
        if (player != null)
            lastPlayerPosition = player.position;
    }

    void Update()
    {
        if (player == null) return;

        // Track custom player velocity
        playerVelocity = (player.position - lastPlayerPosition) / Time.deltaTime;
        lastPlayerPosition = player.position;

        throwTimer += Time.deltaTime;

        if (throwTimer >= throwInterval)
        {
            StartCoroutine(SpawnProjectileAndWarning());
            throwTimer = 0f;
        }
    }

    public IEnumerator SpawnProjectileAndWarning()
    {
        int type = Random.Range(0, 3); // 0 = puddle, 1 = explosive, 2 = big explosive

        Vector3 targetPos = (type == 0) ? player.position : PredictPlayerPosition();
        targetPos.y = 0f;

        GameObject warningPrefab =
            type == 0 ? puddleWarningPrefab :
            type == 1 ? explosiveWarningPrefab :
                        bigExplosiveWarningPrefab;

        GameObject projectilePrefab =
            type == 0 ? puddleProjectilePrefab :
            type == 1 ? explosiveProjectilePrefab :
                        bigExplosiveProjectilePrefab;

        // ?? Spawn warning IMMEDIATELY
        GameObject warning = Instantiate(warningPrefab, targetPos, Quaternion.identity);
        Destroy(warning, warningLifetime);

        yield return null; // Wait one frame just to be safe

        // ?? Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            Vector3 velocity = CalculateArcVelocity(targetPos, throwPoint.position, arcHeight);
            rb.linearVelocity = velocity;
        }
    }

    Vector3 PredictPlayerPosition()
    {
        float distance = Vector3.Distance(throwPoint.position, player.position);
        float estimatedTime = distance / projectileSpeed;
        Vector3 predictedPos = player.position + playerVelocity * estimatedTime;
        predictedPos.y = 0f;
        return predictedPos;
    }

    Vector3 CalculateArcVelocity(Vector3 target, Vector3 origin, float height)
    {
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float gravity = Mathf.Abs(Physics.gravity.y);

        float timeToPeak = Mathf.Sqrt(2 * height / gravity);
        float totalTime = timeToPeak + Mathf.Sqrt(2 * height / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * height);
        Vector3 velocityXZ = toTargetXZ / totalTime;

        return velocityXZ + velocityY;
    }
}
