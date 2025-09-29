using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int currentHealth;

        //To get checkpoint to respawn
        private Player_Respawn respawn;

        private void Start()
        {
            currentHealth = 50;
            respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Respawn>();
        }

        public void TakeDamage(int damage)
        {
                currentHealth -= damage;

            Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
            if (currentHealth <= 0)
            {
                bool Dead = true;
                respawn.DieAndRespawn(Dead);
            }
        }

        //Change Made by AngelR
        // to give him health back from health power up
        public void GetHealth(int GiveHealth)
        {
            if (currentHealth > 0 && currentHealth < maxHealth)
            {
                currentHealth += GiveHealth;
            }
            else if (currentHealth > 100)
            {
            }
        }

    }
}