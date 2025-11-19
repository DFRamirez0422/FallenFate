using UnityEngine;
using NPA_Health_Components;

public class MGRProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public float speed = 20f;
    public int damage = 10;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogWarning("Rigidbody missing on MGRProjectile");

        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        // Move forward manually when kinematic
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    // Spawner calls this to set speed and lifetime
    public void Arm(float newSpeed, float lifetime)
    {
        speed = newSpeed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<Health>(out var hp))
            hp.TakeDamage(damage);

        Destroy(gameObject);
    }
}
