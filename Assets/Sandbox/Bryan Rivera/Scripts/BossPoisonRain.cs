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
    public float portalHeight = 10f; // Height above ground to spawn portals
    public float portalLifetime = 2f;

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
            // Get random position around player
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.y = 0f; // Keep offset on horizontal plane only

            Vector3 spawnPos = player.position + randomOffset;
            spawnPos.y = portalHeight; // Set height AFTER offset is applied

            // Spawn portal
            GameObject portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
            Destroy(portal, portalLifetime);

            // Spawn potion from portal position
            Instantiate(potionPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f); // Small delay between each
        }
    }
}
