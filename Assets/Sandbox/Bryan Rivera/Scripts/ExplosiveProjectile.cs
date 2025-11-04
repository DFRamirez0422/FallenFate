using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour
{
    public GameObject explosivePrefab;
    public float destroyDelay = 0.1f;

    void OnCollisionEnter(Collision collision)
    {
        // Spawn explosive hitbox at the impact point (flattened to ground)
        Vector3 spawnPosition = collision.contacts[0].point;
        spawnPosition.y = 0f;

        Instantiate(explosivePrefab, spawnPosition, Quaternion.identity);

        // Destroy the projectile shortly after impact
        Destroy(gameObject, destroyDelay);
    }
}
