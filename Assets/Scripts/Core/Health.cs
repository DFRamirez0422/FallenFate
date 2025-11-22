using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace NPA_Health_Components
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum HP")]
        [SerializeField] private int maxHealth = 100;
        public int currentHealth;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        [Tooltip("Respawn")]
        [SerializeField] private Player_Respawn _Respawn;
        private readonly string playerTag = "Player";
        private ElenaAI ElenaThrow;

        //Nathan's Iframe variables
        private Renderer rend;
        private Color originalColor;
        private bool iframe, single = false;
        public PlayerController dashScript;
       

        // vvvvv Added by Jose E. from original file. vvvvv //

        private bool m_hasTakenDamage = false;
        private bool m_hasDied = false;

        /// <summary>
        /// Exposed public variable that returns true if the entity was just hit.
        /// </summary>
        public bool IsTakenDamage => m_hasTakenDamage;
       

        /// <summary>
        /// Exposed public variable that returns true if the entity is dead.
        /// </summary>
        public bool IsDead => m_hasDied;

        // ^^^^^ Added by Jose E. from original file. ^^^^^ //

        private void Awake()
        {
            currentHealth = maxHealth; // Initialize health on spawn

            //
            _Respawn = GameObject.FindGameObjectWithTag("RespawnManager").GetComponent<Player_Respawn>();
            ElenaThrow = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();

            rend = GetComponentInChildren<Renderer>();
            if (rend != null)
                originalColor = rend.material.color;

            dashScript = GetComponent<PlayerController>();
        }

        private void Update()
        {
            // Throw PowerUp
            if (Input.GetKeyDown(KeyCode.Z) && this.gameObject.CompareTag(playerTag))
            {
                if (ElenaThrow != null && ElenaThrow.PowerUpHold == 1)
                {
                    ElenaThrow.ThrowPowerUp();
                    ElenaThrow.BackgroundIcon.SetActive(false);
                    ElenaThrow.PowerUpHold = 0;
                }
            }

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            if (iframe)
            {
                if (single == false)
                {
                    single = true;
                    Invoke(nameof(IFrameTimer), 3);
                }
                if (rend != null)
                {
                    Debug.Log("Renderer not null");
                    rend.material.color = Color.blue;
                }
                else
                    Debug.Log("Renderer Null");
            }
            else
            {
                if (rend != null)
                    rend.material.color = originalColor;
                Debug.Log("No iFrame changing color");
            }

            // Tried to add iframes to the dash. didnt work? Idk y.
            //if (dashScript.isDashing = true)
            //{
            //    iframe = true;
            //}
            //else
            //{
            //    iframe = false;
            //}
        }

        private void FixedUpdate()
        {
            m_hasTakenDamage = false; // ADDED BY: Jose E.: default state for this variable.

            // If health drops to 0 or below, kill the object or respawn players
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void TakeDamage(int damage)
        {
            if (iframe == false)
            {
                // Subtract incoming damage from current health
                currentHealth -= damage;
                Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
                m_hasTakenDamage = true; // ADDED BY: Jose E.
                iframe = true;
            }
            else
            {
                Debug.Log("Iframe hit");
            }
        }


        //Change By Angel Rodriguez
        //Heal Player for a certain percent
        public void Heal(float amount)
        {
            if (this.gameObject.CompareTag(playerTag))
            {
                if (currentHealth < maxHealth)
                {
                    float got = maxHealth * amount;
                    currentHealth += (int)got;
                    Debug.Log($"Healed {(int)got}. Health now {currentHealth}");
                }
                else
                {
                    Debug.Log($"Healed 0. Health now {currentHealth}");
                }
            }
        }

        //Heal Player to full health
        public void FullHeal()
        {
            if (this.gameObject.CompareTag(playerTag))
            {
                int got = maxHealth - currentHealth;
                currentHealth += got;
                Debug.Log($"Healed {got}. Health now {currentHealth}");
            }
        }

        public void IFrameTimer()
        {
            iframe = false;
            single = false;
        }
        //Only respawns Player
        //Anything else is destroyed
        private void Die()
        {
            if (this.gameObject.CompareTag(playerTag))
            {
                m_hasDied = true; //ADDED BY: Jose E.
                //this.transform.position = _Respawn.CurrentCheckPoint.transform.position;
                SceneManager.LoadScene("Death");
                currentHealth = 100;
                Debug.Log("Player died");
                Debug.Log("Respawning Player...");
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        // Convenience overloads used by items
        public void HealAbsolute(int amount)
        {
            int before = currentHealth;
            currentHealth = Mathf.Min(currentHealth + Mathf.Max(0, amount), maxHealth);
            Debug.Log($"Healed {currentHealth - before}. HP: {currentHealth}/{maxHealth}");
        }

        public void HealPercent(float percent01)
        {
            percent01 = Mathf.Clamp01(percent01);
            int add = Mathf.CeilToInt(maxHealth * percent01);
            HealAbsolute(add);
        }
    }
}
