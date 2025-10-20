using UnityEngine;
using NPA_Health_Components; // Lets us use the Health script

public class ExplosiveProjectile : MonoBehaviour
{
    // Movement speed and rotation speed for homing behavior
    public float speed = 10f;
    public float rotateSpeed = 5f;

    // Explosion settings
    public float explosionRadius = 4f;
    public int explosionDamage = 40;

    // Visual effect prefab for the explosion
    public GameObject explosionEffectPrefab;

    private Transform target; // The player target to follow

    void Start()
    {
        // Find the player in the scene using their tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    void Update()
    {
        // If no player found, just move straight
        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // Rotate smoothly to follow the player (homing movement)
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        // Move forward in the current direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When projectile hits the player or the ground, explode
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Explode();
            Destroy(gameObject); // Remove the projectile
        }
    }

    void Explode()
    {
        // Create explosion visual effect if assigned
        if (explosionEffectPrefab)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Check for all objects within the explosion radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        // Damage the player if they're within the explosion range
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health health = hit.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(explosionDamage);
            }
        }
    }
}
