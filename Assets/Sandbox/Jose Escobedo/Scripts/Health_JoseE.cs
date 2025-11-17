using UnityEngine;

namespace NPA_Health_Components
{
    public class Health_JoseE : MonoBehaviour
    {
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        private int currentHealth; // Current HP runtime
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        // vvvvv Added by Jose E. from original file. vvvvv //

        /// <summary>
        /// Exposed public variable that returns true if the entity was just hit.
        /// </summary>
        public bool IsTakenDamage = false;

        /// <summary>
        /// Exposed public variable that returns true if the entity is dead.
        /// </summary>
        public bool IsDead = false;

        // ^^^^^ Added by Jose E. from original file. ^^^^^ //

        private void Awake()
        {
            currentHealth = maxHealth; // Initialize health on spawn
        }
        
        public void TakeDamage(int damage)
        {
            // Subtract incoming damage from current health
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
            
            //ADDED BY: Jose E.
            IsTakenDamage = true;

            // If health drops to 0 or below, kill the object
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        public void Heal(int amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            Debug.Log($"Healed {amount}. Health now {currentHealth}");
        }

        private void Die()
        {
            IsDead = true; //ADDED BY: Jose E.
            Debug.Log($"{gameObject.name} has died!");
            Destroy(gameObject);
        }
    }
}