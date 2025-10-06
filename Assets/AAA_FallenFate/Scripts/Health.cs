using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int currentHealth;

        //GetHealth from Elena
        private ElenaAI Elena;

        private void Start()
        {
            currentHealth = 50;
            Elena = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
        }

        private void FixedUpdate()
        {
            if (Elena.CombatToggle && currentHealth < 100/3
                       && Input.GetKeyDown(KeyCode.Z) && Elena.HealthPackHold == 1)
            {
                Elena.ThrowHealthPowerUP();
                Elena.HealthPackHold = 0;
            }
        }

        public void TakeDamage(int damage)
        {
            if(currentHealth <= maxHealth && currentHealth > 0)
                currentHealth -= damage;
            else { }

                Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
        }

        //Change Made by AngelR
        // to give him health back from health power up
        public void GetHealth(int GiveHealth)
        {
            if (currentHealth >= 0 && currentHealth < maxHealth)
            {
                currentHealth += GiveHealth;
            }
            else { }
        }





    }
}