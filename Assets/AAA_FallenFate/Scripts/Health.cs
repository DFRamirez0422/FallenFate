using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;
        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");

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
            Debug.Log($"{gameObject.name} has died!");
            Destroy(gameObject);
        }
    }
}