using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        private int currentHealth; // Current HP runtime
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        [Header("Respawn")]
        bool isdead;
        Player_Respawn _Respawn;

        private void Awake()
        {
            currentHealth = maxHealth; // Initialize health on spawn
            _Respawn =  GameObject.FindGameObjectWithTag("RespawnManager").GetComponent<Player_Respawn>();
        }

        private void FixedUpdate()
        {
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void TakeDamage(int damage)
        {
            // Subtract incoming damage from current health
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            Debug.Log($"Healed {amount}. Health now {currentHealth}");
        }


        //Respawn Player when health is zero
        //Change made by Angel Rodriguez
        private void Die()
        {
            transform.position = _Respawn.CurrentCheckPoint.transform.position;
            currentHealth = 100;
            Debug.Log($"Player has died");
            Debug.Log($"Respawning...");
        }
    }
}