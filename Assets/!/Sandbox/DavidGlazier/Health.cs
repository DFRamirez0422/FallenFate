using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        private int currentHealth; // Current HP runtime
        private void Awake()
        {
            currentHealth = maxHealth; // Initialize health on spawn
        }
        
        public void TakeDamage(int damage)
        {
            // Subtract incoming damage from current health
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");

            // If health drops to 0 or below, kill the object
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} has died!");
            Destroy(gameObject);
        }
    }
}