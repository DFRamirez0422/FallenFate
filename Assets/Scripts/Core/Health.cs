using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;   // (kept from your original — safe to remove if unused at runtime)
using UnityEditor.UIElements;     // (kept from your original — safe to remove if unused at runtime)
using UnityEngine;
using System.Collections;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [Header("Health")]
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        public int MaxHealth => maxHealth;

        [Tooltip("Current HP at runtime")]
        [SerializeField] private int currentHealth;   // was public; keep serialized for inspector
        public int CurrentHealth => currentHealth;

        [Header("Respawn (Player only)")]
        // This is only for player
        [SerializeField] private Player_Respawn _Respawn;
        private TagHandle PlayerTag;

        [Header("External (optional)")]
        private ElenaAI ElenaThrow;

        // -------- Damage Boost (from PlayerStats) --------
        [Header("Damage")]
        [Tooltip("Base damage your player/enemy deals; combine with multiplier via GetOutgoingDamage()")]
        public float baseDamage = 10f;

        [Tooltip("Modified by damage boost. 1 = normal, >1 boosted")]
        public float damageMultiplier = 1f;

        // -------- Shield / Reflect (from PlayerStats) --------
        [Header("Shield / Reflect")]
        [Tooltip("True while shield active; damage ignored, projectiles can be reflected.")]
        public bool isShieldActive;
        private float shieldEndTime;

        private void Awake()
        {
            // Tag system used by your team (Visual Scripting)
            PlayerTag = TagHandle.GetExistingTag("Player");

            // Initialize health on spawn
            currentHealth = maxHealth;

            // Safe-finds (only used for player)
            var respawnGO = GameObject.FindGameObjectWithTag("RespawnManager");
            if (respawnGO)
                _Respawn = respawnGO.GetComponent<Player_Respawn>();

            var elenaGO = GameObject.FindGameObjectWithTag("Elena");
            if (elenaGO)
                ElenaThrow = elenaGO.GetComponent<ElenaAI>();
        }

        private void Update()
        {
            // Your team logic for Z key (player-only)
            if (Input.GetKeyDown(KeyCode.Z) && this.gameObject.CompareTag(PlayerTag))
            {
                if (ElenaThrow != null && ElenaThrow.PowerUpHold == 1)
                {
                    ElenaThrow.ThrowPowerUp();
                    ElenaThrow.PowerUpHold = 0;
                }
            }

            // Clamp health to max
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        private void FixedUpdate()
        {
            // If health drops to 0 or below, kill the object or respawn players
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // ------------------- PUBLIC API -------------------

        /// <summary>
        /// Apply raw damage to this entity. Ignored if shield is active.
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (IsShieldActive())
            {
                Debug.Log($"[{name}] Shield active — no damage taken.");
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - Mathf.Max(0, damage));
            Debug.Log($"[{name}] Took {damage} dmg. HP: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal by a percentage of max health (your team’s original behavior).
        /// Example: 0.25f = heal 25% of max.
        /// </summary>
        public void HealPercent(float percent)
        {
            if (!this.gameObject.CompareTag(PlayerTag)) return;

            if (currentHealth >= maxHealth)
            {
                Debug.Log($"[{name}] Healed 0. Already full: {currentHealth}/{maxHealth}");
                return;
            }

            int delta = Mathf.CeilToInt(maxHealth * Mathf.Max(0f, percent));
            int before = currentHealth;
            currentHealth = Mathf.Min(currentHealth + delta, maxHealth);
            Debug.Log($"[{name}] Healed {currentHealth - before}. HP: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Heal by an absolute amount (useful for item pickups that specify flat HP).
        /// </summary>
        public void HealAbsolute(int amount)
        {
            int delta = Mathf.Max(0, amount);
            int before = currentHealth;
            currentHealth = Mathf.Min(currentHealth + delta, maxHealth);
            Debug.Log($"[{name}] Healed {currentHealth - before}. HP: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Instantly set HP to max (player only per your original logic).
        /// </summary>
        public void FullHeal()
        {
            if (!this.gameObject.CompareTag(PlayerTag)) return;

            int delta = maxHealth - currentHealth;
            currentHealth = maxHealth;
            Debug.Log($"[{name}] Full Heal +{delta}. HP: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Start/refresh a temporary damage boost (e.g., x1.5 for 8s).
        /// </summary>
        public void ApplyDamageBoost(float multiplier, float durationSeconds)
        {
            StopCoroutine(nameof(DamageBoostRoutine));
            StartCoroutine(DamageBoostRoutine(multiplier, durationSeconds));
        }

        private IEnumerator DamageBoostRoutine(float multiplier, float duration)
        {
            damageMultiplier = Mathf.Max(0f, multiplier);
            Debug.Log($"[{name}] Damage boost ON x{damageMultiplier} for {duration}s");
            yield return new WaitForSeconds(Mathf.Max(0f, duration));
            damageMultiplier = 1f;
            Debug.Log($"[{name}] Damage boost OFF");
        }

        /// <summary>
        /// Call this to get the final outgoing damage (base * multiplier) for attacks.
        /// </summary>
        public float GetOutgoingDamage() => baseDamage * damageMultiplier;

        /// <summary>
        /// Activate a shield window where incoming damage is ignored.
        /// (Your projectile logic can check IsShieldActive() to reflect back.)
        /// </summary>
        public void ActivateShieldReflect(float durationSeconds)
        {
            StopCoroutine(nameof(ShieldRoutine));
            StartCoroutine(ShieldRoutine(durationSeconds));
        }

        private IEnumerator ShieldRoutine(float duration)
        {
            isShieldActive = true;
            shieldEndTime = Time.time + Mathf.Max(0f, duration);
            Debug.Log($"[{name}] Shield/Reflect ON for {duration}s");
            // TODO: enable VFX

            while (Time.time < shieldEndTime)
                yield return null;

            isShieldActive = false;
            Debug.Log($"[{name}] Shield/Reflect OFF");
            // TODO: disable VFX
        }

        public bool IsShieldActive() => isShieldActive && Time.time < shieldEndTime;

        // ------------------- INTERNAL -------------------

        private void Die()
        {
            // Only respawns Player — anything else is destroyed (kept from your original)
            if (this.gameObject.CompareTag(PlayerTag))
            {
                if (_Respawn && _Respawn.CurrentCheckPoint)
                {
                    this.transform.position = _Respawn.CurrentCheckPoint.transform.position;
                }
                currentHealth = maxHealth; // restore to full on respawn
                Debug.Log("Player died");
                Debug.Log("Respawning Player...");
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
