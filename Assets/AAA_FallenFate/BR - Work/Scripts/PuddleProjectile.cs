using UnityEngine;

public class PuddleProjectile : MonoBehaviour
{
    // Movement speed and how fast it turns toward the player
    public float speed = 10f;
    public float rotateSpeed = 5f;

    // The puddle prefab that spawns when the projectile hits the ground or player
    public GameObject puddlePrefab;

    private Transform target; // The player to aim at

    void Start()
    {
        // Find the player using their tag when the projectile spawns
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        // If no player found, just move straight forward
        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // Rotate smoothly to face the player (homing movement)
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        // Move forward every frame
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore hitting other puddles (prevents puddle stacking)
        if (other.CompareTag("Puddle"))
            return;

        // Spawn a puddle if it hits the ground or player
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Instantiate(puddlePrefab, transform.position, Quaternion.identity);
        }

        // Destroy the projectile after it hits something
        Destroy(gameObject);
    }
}
