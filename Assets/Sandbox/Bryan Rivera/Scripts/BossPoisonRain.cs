using UnityEngine;
using System.Collections;

public class BossScatterShot : MonoBehaviour
{
    [Header("References")]
    public GameObject portalPrefab;
    public GameObject potionPrefab;
    public Transform player;

    [Header("Attack Settings")]
    public float attackInterval = 10f;
    public int numberOfPortals = 5;
    public float scatterRadius = 5f;
    public float portalHeight = 10f;
    public float portalLifetime = 2f;
    public float minDistanceFromPlayer = 2f; // New: minimum safe distance

    private float timer;

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            StartCoroutine(PerformScatterShot());
            timer = 0f;
        }
    }

    IEnumerator PerformScatterShot()
    {
        for (int i = 0; i < numberOfPortals; i++)
        {
            Vector3 spawnPos;
            int maxAttempts = 10;
            int attempts = 0;

            // Try finding a spawn position outside the safe zone
            do
            {
                Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
                randomOffset.y = 0f;
                spawnPos = player.position + randomOffset;
                attempts++;
            }
            while (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer && attempts < maxAttempts);

            spawnPos.y = portalHeight;

            GameObject portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
            Destroy(portal, portalLifetime);

            Instantiate(potionPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        }
    }

#if UNITY_EDITOR
    // Optional: Draw gizmos in the editor for scatter + safe zone
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer); // Safe zone

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, scatterRadius); // Max range
    }
#endif
}
