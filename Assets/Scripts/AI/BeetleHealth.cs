using UnityEngine.AI;
using UnityEngine;

public class BeetleHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Range(10, 500)] public int maxHealth = 100;
    private int currentHealth;

    [Header("Hitbox Settings")]
    [Range(0.5f, 5f)] public float hitboxRadius = 1f;
    public bool showHitboxGizmo = true;

    [Header("Damage Flash (Optional)")]
    public Renderer beetleRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private bool isFlashing = false;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab;
    public float deathDelay = 0.5f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("=== BEETLE HEALTH START ===");
        Debug.Log("Starting health: " + currentHealth);

        // Get renderer for damage flash
        if (beetleRenderer == null)
        {
            beetleRenderer = GetComponentInChildren<Renderer>();
        }

        if (beetleRenderer != null)
        {
            originalColor = beetleRenderer.material.color;
        }

        // Setup hitbox collider
        SetupHitbox();

        Debug.Log("Beetle setup complete. Current health: " + currentHealth);
    }

    private void SetupHitbox()
    {
        // Check if we already have a collider for hitbox
        Collider[] colliders = GetComponents<Collider>();
        bool hasHitboxCollider = false;

        foreach (Collider col in colliders)
        {
            // Look for a non-trigger collider (the hitbox)
            if (!col.isTrigger)
            {
                hasHitboxCollider = true;

                // Adjust size if it's a sphere collider
                if (col is SphereCollider sphereCol)
                {
                    sphereCol.radius = hitboxRadius;
                }

                if (showDebugLogs)
                    Debug.Log("Hitbox collider found and configured");

                break;
            }
        }

        // If no hitbox collider exists, create one
        if (!hasHitboxCollider)
        {
            SphereCollider hitbox = gameObject.AddComponent<SphereCollider>();
            hitbox.radius = hitboxRadius;
            hitbox.isTrigger = false; // NOT a trigger - this is a physical hitbox

            if (showDebugLogs)
                Debug.Log("Created hitbox collider for beetle");
        }

        // Make sure beetle is tagged as Enemy so it doesn't hit itself
        if (!gameObject.CompareTag("Enemy"))
        {
            Debug.LogWarning("Beetle should be tagged as 'Enemy'!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.LogWarning($"BEETLE TOOK DAMAGE: {damage} | Remaining Health: {currentHealth}/{maxHealth}");

        // Flash damage color
        if (!isFlashing && beetleRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        isFlashing = true;

        if (beetleRenderer != null)
        {
            beetleRenderer.material.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            beetleRenderer.material.color = originalColor;
        }

        isFlashing = false;
    }

    private void Die()
    {
        Debug.LogError("=== BEETLE DIED! ===");
        Debug.LogError("Final health: " + currentHealth);

        // Spawn death effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Disable AI
        AcidSprayBeetleAI ai = GetComponent<AcidSprayBeetleAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        // Disable movement
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Destroy after delay
        Destroy(gameObject, deathDelay);
    }

    // Public methods to check health
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;

    // Visualize hitbox in editor
    private void OnDrawGizmosSelected()
    {
        if (showHitboxGizmo)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Semi-transparent green
            Gizmos.DrawSphere(transform.position, hitboxRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, hitboxRadius);
        }
    }
}