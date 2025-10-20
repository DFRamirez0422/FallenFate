using NPA_RhythmBonusPrefabs;
using UnityEngine;
using NPA_Health_Components;

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
        [SerializeField] private float maxBlockDuration = 1f;

        [Header("Damage Multipliers")]
        [SerializeField] private float normalBlockMultiplier = 0.5f;
        [SerializeField] private float parryMultiplier = 0.1f;

        private bool isBlocking = false;
        private float blockStartTime = 0f;
        private float lastBlockTime = -10f;
        private RhythmBonusJudge.RhythmTier lastBlockTier = RhythmBonusJudge.RhythmTier.Miss;

        void Update()
        {
            // Trigger block only on key down
            if (Input.GetKeyDown(blockKey))
            {
                if (Time.time - lastBlockTime >= blockCooldown)
                {
                    // Start block
                    isBlocking = true;
                    blockStartTime = Time.time;

                    // Evaluate rhythm immediately
                    lastBlockTier = rhythmCounter.EvaluateBeat();

                    if (lastBlockTier == RhythmBonusJudge.RhythmTier.Miss)
                    {
                        Debug.Log("Block Missed! Bad Timing");
                        isBlocking = false; // cancel block
                    }
                    else if (lastBlockTier == RhythmBonusJudge.RhythmTier.Good)
                    {
                        Debug.Log("Normal Block!");
                    }
                    else if (lastBlockTier == RhythmBonusJudge.RhythmTier.Perfect)
                    {
                        Debug.Log("Perfect Parry!");
                    }
                }
            }

            // End block if max duration exceeded
            if (isBlocking && Time.time - blockStartTime >= maxBlockDuration)
            {
                isBlocking = false;
                lastBlockTime = Time.time; // start cooldown
            }

            // Optional: stop block on key release
            if (Input.GetKeyUp(blockKey) && isBlocking)
            {
                isBlocking = false;
                lastBlockTime = Time.time; // start cooldown
            }

            // Update material
            if (playerRenderer != null)
                playerRenderer.material = isBlocking ? blockingMaterial : normalMaterial;
        }

        public void TakeIncomingDamage(int incomingDamage, GameObject attacker)
        {
            int damageToApply = incomingDamage;

            if (isBlocking)
            {
                // Apply damage reduction based on the block tier evaluated at key press
                switch (lastBlockTier)
                {
                    case RhythmBonusJudge.RhythmTier.Perfect:
                        damageToApply = Mathf.RoundToInt(incomingDamage * parryMultiplier);
                        break;

                    case RhythmBonusJudge.RhythmTier.Good:
                        damageToApply = Mathf.RoundToInt(incomingDamage * normalBlockMultiplier);
                        break;

                    case RhythmBonusJudge.RhythmTier.Miss:
                        // Shouldn't happen while isBlocking, but safe fallback
                        break;
                }
            }

            // Apply final damage
            playerHealth.TakeDamage(damageToApply);
        }
    }
}
