using UnityEngine;

public class AcidProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    [Range(1, 100)] public int damage = 15;

    [Header("Hitbox Settings")]
    [Range(0.1f, 2f)] public float hitboxRadius = 0.5f;
    public bool showHitboxGizmo = true;

    [Header("Visual Effects (Optional)")]
    public GameObject hitEffectPrefab;
    public ParticleSystem trailEffect;

    [Header("Audio (Optional)")]
    public AudioClip impactSound;

    private bool hasHit = false;

    private void Start()
    {
        // Make sure we have a collider with correct size
        SphereCollider sphereCol = GetComponent<SphereCollider>();
        CapsuleCollider capsuleCol = GetComponent<CapsuleCollider>();
        MeshCollider meshCol = GetComponent<MeshCollider>();

        if (sphereCol != null)
        {
            sphereCol.radius = hitboxRadius;
            sphereCol.isTrigger = true;
        }
        else if (capsuleCol != null)
        {
            capsuleCol.radius = hitboxRadius;
            capsuleCol.isTrigger = true;
        }
        else if (meshCol != null)
        {
            meshCol.convex = true;
            meshCol.isTrigger = true;
        }
        else
        {
            // No collider found, add one
            Debug.LogWarning("No collider found! Adding SphereCollider...");
            SphereCollider newCol = gameObject.AddComponent<SphereCollider>();
            newCol.radius = hitboxRadius;
            newCol.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        Debug.Log("Projectile hit: " + other.gameObject.name + " (Tag: " + other.tag + ")");

        // Hit the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER! Dealing " + damage + " damage!");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Player hit but no PlayerHealth component found!");
            }

            CreateHitEffect(other.transform.position);
            DestroyProjectile();
        }
        // Hit environment (walls, ground, etc.)
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Debug.Log("Hit environment: " + other.gameObject.name);
            CreateHitEffect(transform.position);
            DestroyProjectile();
        }
    }

    private void CreateHitEffect(Vector3 position)
    {
        // Spawn particle effect
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Play impact sound
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, position, 0.5f);
        }
    }

    private void DestroyProjectile()
    {
        hasHit = true;

        // Detach trail effect so it finishes naturally
        if (trailEffect != null)
        {
            trailEffect.transform.SetParent(null);
            trailEffect.Stop();
            Destroy(trailEffect.gameObject, 2f);
        }

        
    }
}

