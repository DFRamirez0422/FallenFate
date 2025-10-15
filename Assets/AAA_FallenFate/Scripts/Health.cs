using UnityEngine;

namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int currentHealth;

        [Header("InfoForRespawn")]
        public GameObject PLAYER;
        [SerializeField] private bool IsDead;
        private Player_Respawn RespawnPoint;

        //GetHealth from Elena
        private ElenaAI Elena;

        private void Start()
        {
            currentHealth = 50;
            PLAYER = this.gameObject;
            RespawnPoint = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Player_Respawn>();
            Elena = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
        }

        private void FixedUpdate()
        {
            if (IsDead)
            {
                DieAndRespawn();
            }
        }

        public void TakeDamage(int damage)
        {
            if(currentHealth <= maxHealth && currentHealth >= 0)
                currentHealth -= damage;

            if(currentHealth <= 0)
            {
                IsDead = true;
            }
        }

        //Change Made by AngelR
        // to give him health back from health power up
        public void GetHealth(int GiveHealth)
        {
            if (currentHealth >= 0 && currentHealth < maxHealth)
            {
                currentHealth += GiveHealth;
            }
        }

        public void DieAndRespawn()
        {
            Debug.Log($"{gameObject.name} has died!");
            if (IsDead)
            {
                this.transform.localPosition = RespawnPoint.CurrentRespawnPoint.position;
                IsDead = false;
                currentHealth = 100;
            }
        }
    }
}