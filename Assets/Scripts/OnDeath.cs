using System.Collections;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    [Tooltip("Delay in seconds before the object is destroyed after death.")]
    [SerializeField] private float deathDelay = 0.5f;

    public void Die()
    {
        Debug.Log($"{name} died");

        // Disable AI / chasing
        GetComponent<DummyEnemy>()?.SetStunned(true);

        // Delay physics shutdown so knockback can play
        StartCoroutine(DeathDelay());
    }

    // --- Coroutine to handle death delay so knockback works on final blow ---
    private IEnumerator DeathDelay()
    {
        // Wait for knockback time to finish
        EnemyKnockback knockback = GetComponent<EnemyKnockback>();
        float delay = knockback != null ? knockback.KnockbackTime : 0f;

        yield return new WaitForSeconds(delay);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        Destroy(gameObject, deathDelay);
    }
}