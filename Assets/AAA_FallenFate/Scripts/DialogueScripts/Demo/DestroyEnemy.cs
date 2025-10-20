using UnityEngine;
using System.Collections;

public class DestroyEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float respawnDelay = 5f;
    public GameObject enemyPrefab; // Assign prefab for respawning
    public string enemyTag = "Enemy"; // Tag used for enemies

    [Header("Input Settings")]
    public KeyCode destroyKey = KeyCode.F; // Key to destroy enemy

    // Event for enemy destroyed
    public static event System.Action<Vector3> OnEnemyDestroyed;

    void Update()
    {
        if (Input.GetKeyDown(destroyKey))
        {
            DestroyClosestEnemy();
        }
    }

    /// <summary>
    /// Finds the closest enemy and destroys it
    /// </summary>
    private void DestroyClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0)
        {
            Debug.Log("No enemies found to destroy.");
            return;
        }

        // Find the closest enemy to this DestroyEnemy object (usually player)
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(currentPos, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        if (closest != null)
        {
            DestroyEnemyInstance(closest);
        }
    }

    /// <summary>
    /// Call this to destroy an enemy in the scene
    /// </summary>
    public void DestroyEnemyInstance(GameObject enemy)
    {
        Vector3 position = enemy.transform.position;

        // Destroy the enemy
        Destroy(enemy);

        // Fire the event so NPCs react
        OnEnemyDestroyed?.Invoke(position);

        // Respawn if prefab assigned
        if (enemyPrefab != null)
            StartCoroutine(RespawnEnemy(position));
    }

    private IEnumerator RespawnEnemy(Vector3 position)
    {
        yield return new WaitForSeconds(respawnDelay);
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}