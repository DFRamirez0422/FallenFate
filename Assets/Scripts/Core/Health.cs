using Unity.Burst;
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
            if(currentHealth > MaxHealth)
            {
                currentHealth = MaxHealth;
            }
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



        //Diffrenet Types of healing
        //Change Made by AngelRodriguez
        public void Heal(float amount)
        {
            float HealthGotten = maxHealth * amount;
            currentHealth += (int)HealthGotten; 
            
            if(currentHealth > MaxHealth)
            {
                Debug.Log($"Healed {(int)HealthGotten}. Health now {MaxHealth}/{MaxHealth}");
            }
            else
            {
                Debug.Log($"Healed {(int)HealthGotten}. Health now {currentHealth}/{MaxHealth}");
            }
        }

        public void HalfHeal(float amount)
        {
            float HealthGotten = maxHealth * amount;
            currentHealth += (int)HealthGotten;
            if (currentHealth > MaxHealth)
            {
                Debug.Log($"Healed {(int)HealthGotten}. Health now {MaxHealth}/{MaxHealth}");
            }
            else
            {
                Debug.Log($"Healed {(int)HealthGotten}. Health now {currentHealth}/{MaxHealth}");
            }
        }

        public void FullHeal()
        {
            int HealthGotten = maxHealth - currentHealth;
            currentHealth += maxHealth - currentHealth;
            if (currentHealth > MaxHealth)
            {
                Debug.Log($"Healed {HealthGotten}. Health now {MaxHealth}/{MaxHealth}");
            }
            else
            {
                Debug.Log($"Healed {HealthGotten}. Health now {currentHealth}/{MaxHealth}");
            }
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