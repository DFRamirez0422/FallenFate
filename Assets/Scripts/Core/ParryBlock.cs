
using UnityEngine;
using NPA_Health_Components;
using NPA_PlayerPrefab.Scripts;

namespace AAA_FallenFate.Scripts.PlayerScripts
{
    public class ParryBlock : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health playerHealth;
        [SerializeField] private Renderer playerRenderer;
        [SerializeField] private PlayerCombat playerAttack; // disable attacks while blocking

        [Header("Materials")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material blockingMaterial;
        [SerializeField] private Material parryMaterial;

        [Header("Block / Parry Settings")]
        [SerializeField] private KeyCode blockKey = KeyCode.Mouse1;
        [SerializeField] private float parryWindow = 0.25f;       // how long after pressing is considered a parry
        [SerializeField] private float blockMaxDuration = 1.5f;   // how long you can hold block
        [SerializeField] private float blockCooldown = 0.75f;     // cooldown starts on release
        [SerializeField] private float blockDamageMultiplier = 0.5f;
        [SerializeField] private float parryDamageMultiplier = 0f;
        [SerializeField] private float enemyStunDuration = 0.5f;

        // vvvvv Added by Jose E. from original file. vvvvv //

        /// <summary>
        /// Expoed public variable that returns whether or not the player is currently in a perry block.
        /// </summary>
        public bool IsBlocking => isBlocking;

        // ^^^^^ Added by Jose E. from original file. ^^^^^ //

        private bool isBlocking = false;
        private float blockStartTime = -10f;
        private float lastBlockReleaseTime = -10f;
        private float parryStartTime = -10f;

        void Update()
        {
            bool onCooldown = Time.time - lastBlockReleaseTime < blockCooldown;

            // --- Block / Parry input ---
            if (!onCooldown)
            {
                if (Input.GetKeyDown(blockKey))
                {
                    // Press = Parry Timing
                    parryStartTime = Time.time;
                    StartBlock();
                }

                if (isBlocking)
                {
                    // Check max duration
                    if (Time.time - blockStartTime > blockMaxDuration)
                    {
                        EndBlock(false);
                    }
                }

                if (Input.GetKeyUp(blockKey) && isBlocking)
                {
                    EndBlock(true); // releasing starts cooldown
                }
            }
            else
            {
                isBlocking = false;
            }

            // --- Disable attacking while blocking ---
            if (playerAttack != null)
                playerAttack.enabled = !isBlocking;

            // --- Material feedback ---
            if (playerRenderer != null)
            {
                if (onCooldown)
                {
                    playerRenderer.material = normalMaterial;
                }
                else if (IsInParryWindow())
                {
                    playerRenderer.material = parryMaterial;
                }
                else if (isBlocking)
                {
                    playerRenderer.material = blockingMaterial;
                }
                else
                {
                    playerRenderer.material = normalMaterial;
                }
            }
        }

        private void StartBlock()
        {
            isBlocking = true;
            blockStartTime = Time.time;
        }

        private void EndBlock(bool releasedNormally)
        {
            isBlocking = false;
            if (releasedNormally)
            {
                lastBlockReleaseTime = Time.time; // cooldown starts on release
            }
        }

        public void TakeIncomingDamage(int incomingDamage, GameObject attacker)
        {
            int finalDamage = incomingDamage;

            if (IsInParryWindow())
            {
                // ✅ Parry happens if damage comes right after press
                finalDamage = Mathf.RoundToInt(incomingDamage * parryDamageMultiplier);
                Debug.Log("Parry!");

                if (attacker != null)
                {
                    var stun = attacker.GetComponent<Hitstun>();
                    if (stun != null)
                        stun.ApplyHitstun(enemyStunDuration);
                }
            }
            else if (isBlocking)
            {
                // Normal block
                finalDamage = Mathf.RoundToInt(incomingDamage * blockDamageMultiplier);
                Debug.Log("Blocked!");
            }

            playerHealth.TakeDamage(finalDamage);
        }

        private bool IsInParryWindow()
        {
            return Time.time - parryStartTime <= parryWindow;
        }
    }
}
