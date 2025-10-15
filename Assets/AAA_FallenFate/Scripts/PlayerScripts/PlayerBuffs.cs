using NPA_Health_Components;
using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerBuffs : MonoBehaviour
    {
        private float lifestealPercent = 0f;
        private float lifestealTimer = 0f;
        private Health health;

        public bool IsLifestealActive => lifestealTimer > 0f;
        void Awake()
        {
            health = GetComponent<Health>();
            if (health == null)
                Debug.LogWarning("PlayerBuffs: No Health component found on player!");
        }

        void Update()
        {
            if (lifestealTimer > 0)
            {
                lifestealTimer -= Time.deltaTime;
                if (lifestealTimer <= 0) lifestealPercent = 0f;
            }
        }

        public void ApplyLifesteal(float percent, float duration)
        {
            lifestealPercent = percent;
            lifestealTimer = duration;
            Debug.Log($"Lifesteal applied: {percent*100}% for {duration}s");
        }

        public void HealFromDamage(int damage)
        {
            if (lifestealPercent > 0 && health != null)
            {
                int healAmount = Mathf.RoundToInt(damage * lifestealPercent);
                health.Heal(healAmount);
                Debug.Log($"Healed {healAmount} from lifesteal.");
            }
        }
    }
}

  

