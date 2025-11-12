using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEditor.UIElements;
using UnityEngine;

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
        private TagHandle PlayerTag;
        private ElenaAI ElenaThrow;

        private void Awake()
        {
            PlayerTag = TagHandle.GetExistingTag("Player");
            currentHealth = maxHealth;
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

            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }

        private void FixedUpdate()
        {
            if (currentHealth <= 0) Die();
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= Mathf.Max(0, damage);
            Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");
        }

        public void Heal(float amount)
        {
            if (this.gameObject.CompareTag(PlayerTag))
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

        public void FullHeal()
        {
            if (this.gameObject.CompareTag(PlayerTag))
            {
                int got = maxHealth - currentHealth;
                currentHealth += got;
                Debug.Log($"Healed {got}. Health now {currentHealth}");
            }
        }

        private void Die()
        {
            if (this.gameObject.CompareTag(PlayerTag))
            {
                this.transform.position = _Respawn.CurrentCheckPoint.transform.position;
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
