using NPA_RhythmBonusPrefabs;
using UnityEngine;
using NPA_Health_Components; // so we can talk to Health

namespace AAA_FallenFate.Scripts.PlayerScripts
{
    public class ParryBlock : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BeatComboCounter rhythmCounter;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Renderer playerRenderer;

        [Header("Materials")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material blockingMaterial;

        [Header("Block Settings")]
        [SerializeField] private KeyCode blockKey = KeyCode.Mouse1;
        [SerializeField] private float blockCooldown = 0.5f;

        [Header("Damage Multipliers")]
        [SerializeField] private float normalBlockMultiplier = 0.5f;
        [SerializeField] private float parryMultiplier = 0.1f;

        private bool isBlocking = false;
        private float lastBlockTime = -10f;

        void Update()
        {
            // Check if block key is held
            isBlocking = Input.GetKey(blockKey);

            // Change material for visual feedback
            if (playerRenderer != null)
                playerRenderer.material = isBlocking ? blockingMaterial : normalMaterial;
        }

        /// <summary>
        /// Call this instead of Health.TakeDamage directly.
        /// </summary>
        public void TakeIncomingDamage(int incomingDamage)
        {
            int damageToApply = incomingDamage;

            if (isBlocking)
            {
                // enforce cooldown
                if (Time.time - lastBlockTime >= blockCooldown)
                {
                    lastBlockTime = Time.time;

                    var tier = rhythmCounter.EvaluateBeat();
                    switch (tier)
                    {
                        case RhythmBonusJudge.RhythmTier.Perfect:
                            damageToApply = Mathf.RoundToInt(incomingDamage * parryMultiplier);
                            Debug.Log("Perfect Parry!");
                            break;

                        case RhythmBonusJudge.RhythmTier.Good:
                            damageToApply = Mathf.RoundToInt(incomingDamage * normalBlockMultiplier);
                            Debug.Log("Normal Block!");
                            break;

                        case RhythmBonusJudge.RhythmTier.Miss:
                            Debug.Log("Block Missed!");
                            break;
                    }
                }
            }

            // Pass final damage to Health
            playerHealth.TakeDamage(damageToApply);
        }
    }
}
