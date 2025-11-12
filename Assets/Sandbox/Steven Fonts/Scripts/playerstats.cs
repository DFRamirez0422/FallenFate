using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Damage")]
    public float baseDamage = 10f;
    public float damageMultiplier = 1f;

    [Header("Shield / Reflect")]
    public bool isShieldActive;
    private float shieldEndTime;

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[PlayerStats] Healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StopCoroutine(nameof(DamageBoostRoutine));
        StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    private IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        Debug.Log($"[PlayerStats] Damage boost ON x{multiplier} for {duration}s");
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        Debug.Log("[PlayerStats] Damage boost OFF");
    }

    public float GetOutgoingDamage() => baseDamage * damageMultiplier;

    // ---------- NEW: Take Damage + Shield ----------
    public void TakeDamage(float amount)
    {
        if (IsShieldActive())
        {
            Debug.Log("[PlayerStats] Shield active — no damage taken.");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"[PlayerStats] Took {amount} dmg. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            // TODO: death/respawn handling
            Debug.Log("[PlayerStats] Player died (stub).");
        }
    }

    public void ActivateShieldReflect(float durationSeconds)
    {
        StopCoroutine(nameof(ShieldRoutine));
        StartCoroutine(ShieldRoutine(durationSeconds));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        isShieldActive = true;
        shieldEndTime = Time.time + duration;
        Debug.Log($"[PlayerStats] Shield/Reflect ON for {duration}s");
        // Optional: enable a visual effect here

        while (Time.time < shieldEndTime)
            yield return null;

        isShieldActive = false;
        Debug.Log("[PlayerStats] Shield/Reflect OFF");
        // Optional: disable VFX here
    }

    public bool IsShieldActive() => isShieldActive && Time.time < shieldEndTime;
}
