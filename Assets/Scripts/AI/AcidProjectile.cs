using UnityEngine;

public class AcidProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    [Range(1, 100)] public int damage = 15;

    [Header("Visual Effects (Optional)")]
    public GameObject hitEffectPrefab;
    public ParticleSystem trailEffect;

    [Header("Audio (Optional)")]
    public AudioClip impactSound;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Hit the player
        if (other.CompareTag("Player"))
        {
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

        Destroy(gameObject);
    }
}