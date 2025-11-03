using UnityEngine;

public class PotionProjectile : MonoBehaviour
{
    public GameObject puddlePrefab;
    public float destroyDelay = 0.1f;

    void OnCollisionEnter(Collision collision)
    {
        // Spawn puddle at the impact point (flattened to ground)
        Vector3 spawnPosition = collision.contacts[0].point;
        spawnPosition.y = 0f;

        Instantiate(puddlePrefab, spawnPosition, Quaternion.identity);

        // Destroy the potion after a tiny delay
        Destroy(gameObject, destroyDelay);
    }
}
