using System.Collections;
using UnityEngine;

public class PowerUpeffects : MonoBehaviour
{
    [Header("Damage")]
    public float baseDamage = 10f;
    public float damageMultiplier = 1f;

    [Header("Shield / Reflect")]
    [SerializeField] private bool isShieldActive;
    [SerializeField] private float shieldEndTime;

        private NPA_Health_Components.Health health;

    void Awake()
    {
        health = GetComponent<NPA_Health_Components.Health>();
    }

    public void ApplyDamageBoost(float multiplier, float durationSeconds)
    {
        StopCoroutine(nameof(DamageBoostRoutine));
        StartCoroutine(DamageBoostRoutine(multiplier, durationSeconds));
    }

    public float GetOutgoingDamage() => baseDamage * Mathf.Max(0.01f, damageMultiplier);

    public void ActivateShieldReflect(float durationSeconds)
    {
        StopCoroutine(nameof(ShieldRoutine));
        StartCoroutine(ShieldRoutine(durationSeconds));
    }

    public bool IsShieldActive() => isShieldActive && Time.time < shieldEndTime;

    public void SafeTakeDamage(int amount)
    {
        if (IsShieldActive()) return;
        health.TakeDamage(Mathf.Max(0, amount));
    }

    private IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier <= 0f ? 1f : multiplier;
        Debug.Log($"[CombatEffects] Damage Boost ACTIVATED � x{damageMultiplier} for {duration:F1} seconds.");
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        Debug.Log("[CombatEffects] Damage Boost ENDED � back to normal.");
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        isShieldActive = true;
        shieldEndTime = Time.time + duration;
        Debug.Log($"[CombatEffects] Shield/Reflect ACTIVATED for {duration:F1} seconds.");
        while (Time.time < shieldEndTime) yield return null;
        isShieldActive = false;
        Debug.Log("[CombatEffects] Shield/Reflect ENDED.");
    }

            public void Heal(float amount)
            {
            float HealthGotten = health.MaxHealth * amount;
            health.currentHealth += (int)HealthGotten;
            if (health.currentHealth > health.MaxHealth)
            {
                health.currentHealth = health.MaxHealth;
            }
            Debug.Log($"Healed {(int)HealthGotten}. HP: {health.currentHealth}/{health.MaxHealth}");
            }

        //Heal Player to full health
        public void FullHeal()
        {
        int healthGotten = health.MaxHealth - health.currentHealth;
        health.currentHealth = health.MaxHealth;
        Debug.Log($"Healed {healthGotten}. HP: {health.currentHealth}/{health.MaxHealth}");
        }
}
