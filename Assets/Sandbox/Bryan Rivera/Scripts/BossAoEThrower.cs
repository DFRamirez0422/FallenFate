using UnityEngine;
using System.Collections;
using NPA_PlayerPrefab.Scripts;

public class BossAoEThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject puddleWarningPrefab;
    public GameObject explosiveWarningPrefab;
    public GameObject puddleProjectilePrefab;
    public GameObject explosiveProjectilePrefab;
    public Transform throwPoint;
    public Transform player;

    [Header("Settings")]
    public float throwInterval = 3f;
    public float arcHeight = 5f;
    public float desiredTimeToImpact = 1f;
    public float maxPredictionDistance = 5f;

    [Header("Visual Fixes")]
    public float warningYOffset = 0.02f;

    private float throwTimer = 0f;
    private Vector3 lastPlayerPosition;
    private Vector3 smoothedPlayerVelocity;

    void Start()
    {
        if (player != null)
            lastPlayerPosition = player.position;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 currentVelocity = (player.position - lastPlayerPosition) / Time.deltaTime;
        smoothedPlayerVelocity = Vector3.Lerp(smoothedPlayerVelocity, currentVelocity, 0.1f);

        lastPlayerPosition = player.position;
        throwTimer += Time.deltaTime;

        if (throwTimer >= throwInterval)
        {
            StartCoroutine(SpawnWarningAndThrow());
            throwTimer = 0f;
        }
    }

    IEnumerator SpawnWarningAndThrow()
    {
        bool usePuddle = Random.value > 0.5f;

        Vector3 targetPos = usePuddle ? player.position : PredictPlayerPosition();

        Vector3 warningSpawnPos = new Vector3(targetPos.x, warningYOffset, targetPos.z);
        GameObject warningPrefab = usePuddle ? puddleWarningPrefab : explosiveWarningPrefab;
        GameObject warning = Instantiate(warningPrefab, warningSpawnPos, Quaternion.identity);

        GameObject projectilePrefab = usePuddle ? puddleProjectilePrefab : explosiveProjectilePrefab;
        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            Vector3 velocity = CalculateArcVelocityWithTime(targetPos, throwPoint.position, arcHeight, desiredTimeToImpact);
            rb.linearVelocity = velocity;
        }

        DestroyWarningOnImpact destroyScript = projectile.GetComponent<DestroyWarningOnImpact>();
        if (destroyScript != null)
        {
            destroyScript.SetWarning(warning);
        }

        yield return null;
    }

    Vector3 PredictPlayerPosition()
    {
        Vector3 predictedOffset = smoothedPlayerVelocity * desiredTimeToImpact;
        predictedOffset = Vector3.ClampMagnitude(predictedOffset, maxPredictionDistance);

        Vector3 predictedPos = player.position + predictedOffset;
        return predictedPos;
    }

    Vector3 CalculateArcVelocityWithTime(Vector3 target, Vector3 origin, float height, float time)
    {
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);

        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 velocityXZ = toTargetXZ / time;

        float displacementY = target.y - origin.y;
        float velocityY = (displacementY + 0.5f * gravity * time * time) / time;

        return velocityXZ + Vector3.up * velocityY;
    }
}
