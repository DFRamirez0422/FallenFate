using System.Collections;
using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerCombatImproved : MonoBehaviour, IPlayerCombat
    {
        [Header("References")]
        [SerializeField] private GameObject hitBoxPrefab;           // Hitbox prefab
        [SerializeField] private AttackData defaultAttack;          // Attack stats
        [SerializeField] private PlayerController playerController; // Reads facing & dash state
        [SerializeField] private AttackData dashAttackData;         // Special dash attack data
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0; // Basic attack button
        
        [Header("Combo Settings")]
        [SerializeField] private AttackData[] comboAttacks; // assign 1–4 moves
        [SerializeField] private float attackCooldown = 0.25f;
        [SerializeField] private float comboResetDelay = 1f;
        
        [Header("Attack Flow Settings")]
        [SerializeField] private bool enableAttackCanceling = true;  // Allow recovery canceling
        [SerializeField] private bool enableInputBuffering = true;   // Queue next attack
        [SerializeField] private float bufferWindow = 0.2f;          // How early you can buffer
        
        // --- Combo Finisher System ---
        [SerializeField] private KeyCode finisherKey = KeyCode.Mouse1; // Finisher button
        [SerializeField] private AttackData finisher3Hit;
        [SerializeField] private AttackData finisher6Hit;
        [SerializeField] private AttackData finisher9Hit;

        private int currentHitCount = 0; // Tracks consecutive successful hits

        private int currentComboStep = 0; 
        private float nextAttackTime = 0f;
        private float lastAttackTime = 0f;

        private bool isAttacking = false; // Prevents attack spamming
        private bool isInRecovery = false; // Track if in recovery phase
        
        // Input buffering
        private bool attackBuffered = false;
        private bool finisherBuffered = false;
        private float bufferTimeStamp = 0f;

        [Header("Debug (ONLY FOR TESTING)")]
        [Tooltip("Text object to display the current player state.")]
        [SerializeField] private PlayerDebugUI m_DebugUI;
        
        void Update()
        {
            HandleAttackInput();
            HandleFinisherInput();
            ProcessBufferedInputs();
        }

        private void HandleAttackInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                // Can attack immediately if not attacking or cooldown passed
                if (!isAttacking && Time.time >= nextAttackTime)
                {
                    PerformAttack();
                }
                // Can cancel recovery with next combo attack
                else if (enableAttackCanceling && isInRecovery)
                {
                    PerformAttack();
                }
                // Buffer the input if buffering is enabled
                else if (enableInputBuffering && isAttacking && !attackBuffered)
                {
                    attackBuffered = true;
                    bufferTimeStamp = Time.time;
                }
            }
        }

        private void HandleFinisherInput()
        {
            if (Input.GetKeyDown(finisherKey))
            {
                // Try to use finisher immediately
                if (!isAttacking && TryUseFinisher())
                {
                    return;
                }
                // Buffer finisher input
                else if (enableInputBuffering && isAttacking && !finisherBuffered)
                {
                    finisherBuffered = true;
                    bufferTimeStamp = Time.time;
                }
            }
        }

        private void ProcessBufferedInputs()
        {
            // Only process if not currently attacking and within buffer window
            if (isAttacking || Time.time - bufferTimeStamp > bufferWindow)
            {
                // Clear expired buffers
                if (Time.time - bufferTimeStamp > bufferWindow)
                {
                    attackBuffered = false;
                    finisherBuffered = false;
                }
                return;
            }

            // Process finisher first (higher priority)
            if (finisherBuffered)
            {
                finisherBuffered = false;
                if (TryUseFinisher())
                {
                    attackBuffered = false; // Clear attack buffer too
                    return;
                }
            }

            // Process attack buffer
            if (attackBuffered)
            {
                attackBuffered = false;
                PerformAttack();
            }
        }

        private void PerformAttack()
        {
            AttackData attackData;

            // DASH ATTACK LOGIC
            if (playerController.DashAttackWindowActive)
            {
                attackData = dashAttackData;
                playerController.ConsumeDashAttack();
            }
            else
            {
                // COMBO ATTACK LOGIC
                // If too much time has passed since last combo attack, reset to first combo step
                if (Time.time - lastAttackTime > comboResetDelay)
                    currentComboStep = 0;

                // Pick the attack in the combo sequence based on current step
                attackData = comboAttacks[Mathf.Clamp(currentComboStep, 0, comboAttacks.Length - 1)];

                // Advance combo step for next attack
                currentComboStep++;

                // Track time of this attack for combo reset logic
                lastAttackTime = Time.time;
            }

            // PERFORM ATTACK
            Attack(attackData);

            // SET COOLDOWN
            nextAttackTime = Time.time + attackData.cooldown;
        }

        private bool TryUseFinisher()
        {
            AttackData chosenFinisher = null;

            if (currentHitCount >= 9) chosenFinisher = finisher9Hit;
            else if (currentHitCount >= 6) chosenFinisher = finisher6Hit;
            else if (currentHitCount >= 3) chosenFinisher = finisher3Hit;

            if (chosenFinisher != null)
            {
                Attack(chosenFinisher);
                currentHitCount = 0; // Reset hit counter after using finisher
                return true;
            }

            return false;
        }
        
        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null) return;

            // If we're canceling recovery, stop the current coroutine
            if (isInRecovery)
            {
                StopAllCoroutines();
                playerController.SetAttackLock(false);
            }

            isAttacking = true;
            isInRecovery = false;
            playerController.SetAttackLock(true); // Freeze or slow movement

            Vector3 facing = playerController.FacingDirection;
            Quaternion facingRot = Quaternion.LookRotation(facing, Vector3.up)
                                   * Quaternion.Euler(attackData.rotationOffset);
            Vector3 spawnPos = transform.position + facingRot * attackData.hitboxOffset;

            // Delay hitbox spawn until after startup
            StartCoroutine(HandleAttackPhases(attackData, spawnPos, facingRot, facing));
        }

        private IEnumerator HandleAttackPhases(AttackData attackData, Vector3 spawnPos, Quaternion rot, Vector3 facing)
        {
            // STARTUP
            yield return new WaitForSeconds(attackData.startupTime);

            GameObject hb = null;
            GameObject hbProj = null;

            // ACTIVE
            if (attackData.projectilePrefab == null)
            {
                // Only spawn hitbox
                hb = Instantiate(hitBoxPrefab, spawnPos, rot, transform);
                if (hb.TryGetComponent<Hitbox>(out Hitbox hbComp))
                {
                    hbComp.Initialize(attackData, this.gameObject);
                    hbComp.SetOwnerCombat(this);
                }
            }
            else
            {
                // Only spawn projectile
                hbProj = Instantiate(attackData.projectilePrefab, spawnPos, rot);

                if (hbProj.TryGetComponent<Hitbox>(out Hitbox hbProjComp))
                {
                    hbProjComp.Initialize(attackData, this.gameObject);
                    hbProjComp.SetOwnerCombat(this);
                }

                if (hbProj.TryGetComponent<ProjectileMover>(out ProjectileMover mover))
                {
                    mover.direction = facing.normalized;
                    mover.speed = attackData.projectileSpeed;
                    
                    hbProj.transform.rotation = Quaternion.LookRotation(facing, Vector3.up);
                }
            }

            // Keep hitbox alive for activeTime
            yield return new WaitForSeconds(attackData.activeTime);

            if (hb != null) Destroy(hb);
            if (hbProj != null) Destroy(hbProj);

            // RECOVERY - can be canceled if enableAttackCanceling is true
            isInRecovery = true;
            yield return new WaitForSeconds(attackData.recoveryTime);

            // Only unlock if we weren't canceled
            if (isInRecovery)
            {
                isAttacking = false;
                isInRecovery = false;
                playerController.SetAttackLock(false);

                // Reset Combo
                if (currentComboStep >= comboAttacks.Length)
                    currentComboStep = 0;
            }
        }
        
        public void RegisterHit()
        {
            currentHitCount++;

            Debug.Log($"Hit Count: {currentHitCount}");

            // Example: unlock finishers
            if (currentHitCount == 3)
                Debug.Log("3-Hit Finisher unlocked!");
            else if (currentHitCount == 6)
                Debug.Log("6-Hit Finisher unlocked!");
            else if (currentHitCount == 9)
                Debug.Log("9-Hit Finisher unlocked!");
        }
    }
}
