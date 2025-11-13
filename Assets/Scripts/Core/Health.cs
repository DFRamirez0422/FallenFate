using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        public int currentHealth; // Current HP runtime
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;  

        [Tooltip("Respawn")]
        //This is only for player
        [SerializeField] private Player_Respawn _Respawn;
        private TagHandle PlayerTag;
        private ElenaAI ElenaThrow;

        private void Awake()
        {
            PlayerTag = TagHandle.GetExistingTag("Player");
            currentHealth = maxHealth; // Initialize health on spawn
            _Respawn = GameObject.FindGameObjectWithTag("RespawnManager").GetComponent<Player_Respawn>();
            ElenaThrow = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && this.gameObject.CompareTag(PlayerTag))
            {
                if (ElenaThrow != null && ElenaThrow.PowerUpHold == 1)
                {
                    ElenaThrow.ThrowPowerUp();
                    ElenaThrow.PowerUpHold = 0;
                }
            }

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

        }
        private void FixedUpdate()
        {
            // If health drops to 0 or below, kill the object or respawn players
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

        //Change By Angel Rodriguez
        //Heal Player for a certain percent
        public void Heal(float amount)
        {
            if (this.gameObject.CompareTag(PlayerTag))
            {
                if (currentHealth < maxHealth)
                {
                    float HealthGotten = maxHealth * amount;
                    currentHealth += (int)HealthGotten;
                    Debug.Log($"Healed {(int)HealthGotten}. Health now {currentHealth}");
                }
                else {
                    int HealthGotten = 0;
                    Debug.Log($"Healed {HealthGotten}. Health now {currentHealth}"); 
                }
            }
        }

        //Heals Player to full Health
        public void FullHeal()
        {
            if (this.gameObject.CompareTag(PlayerTag))
            {
                int HealthGotten = maxHealth - currentHealth;
                currentHealth += HealthGotten;
                Debug.Log($"Healed {HealthGotten}. Health now {currentHealth}");
            }
        }


        //Only respawns Player
        //Anything else is destroyed
        private void Die()
        {
            if (this.gameObject.CompareTag(PlayerTag))
            {
                //this.transform.position = _Respawn.CurrentCheckPoint.transform.position;
                SceneManager.LoadScene("Death");
                currentHealth = 100;
                Debug.Log("Player died");
                Debug.Log("Respawning Player...");
            }
            else {
                var copy = this.gameObject;
                Destroy(copy);
            }
        }
    }
}