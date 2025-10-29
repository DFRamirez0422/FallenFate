using System.Collections;
using UnityEngine;
using NPA_RhythmBonusPrefabs;

namespace NPA_PlayerPrefab.Scripts
{
    /// <summary>
    /// Rhythm-enhanced combat system that rewards on-beat attacks with faster cancels
    /// and punishes off-beat attacks with sluggish recovery
    /// </summary>
    public class PlayerCombatRhythm : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject hitBoxPrefab;
        [SerializeField] private AttackData defaultAttack;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AttackData dashAttackData;
        [SerializeField] private RhythmBonusJudge rhythmJudge; // Rhythm system reference
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        
        [Header("Combo Settings")]
        [SerializeField] private AttackData[] comboAttacks;
        [SerializeField] private float attackCooldown = 0.25f;
        [SerializeField] private float comboResetDelay = 1f;
        
        [Header("Rhythm Timing Modifiers")]
        [Tooltip("Recovery time multiplier on Perfect hit")]
        [SerializeField] private float perfectRecoveryMult = 0.5f;  // 50% faster recovery
        
        [Tooltip("Recovery time multiplier on Good hit")]
        [SerializeField] private float goodRecoveryMult = 0.75f;    // 25% faster recovery
        
        [Tooltip("Recovery time multiplier on Miss")]
        [SerializeField] private float missRecoveryMult = 1.5f;     // 50% slower recovery
        
        [Tooltip("Can cancel recovery early on Perfect/Good hits")]
        [SerializeField] private bool rhythmCancelEnabled = true;
        
        [Tooltip("Visual feedback for rhythm hits")]
        [SerializeField] private bool showRhythmFeedback = true;
        
        [Header("Attack Flow Settings")]
        [SerializeField] private bool enableAttackCanceling = true;
        [SerializeField] private bool enableInputBuffering = true;
        [SerializeField] private float bufferWindow = 0.2f;
        
        [Header("Combo Finisher System")]
        [SerializeField] private KeyCode finisherKey = KeyCode.Mouse1;
        [SerializeField] private AttackData finisher3Hit;
        [SerializeField] private AttackData finisher6Hit;
        [SerializeField] private AttackData finisher9Hit;

        private int currentHitCount = 0;
        private int currentComboStep = 0;
        private float nextAttackTime = 0f;
        private float lastAttackTime = 0f;
        
        private bool isAttacking = false;
        private bool isInRecovery = false;
        
        // Rhythm state
        private RhythmBonusJudge.RhythmTier lastRhythmTier = RhythmBonusJudge.RhythmTier.Miss;
        private float currentRecoveryModifier = 1f;
        
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
            UpdateDebugUi(); // <--- TODO: remove when debugging code is finished
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
                // Can cancel recovery with next combo attack (rhythm-based)
                else if (enableAttackCanceling && isInRecovery && CanRhythmCancel())
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
                    attackBuffered = false;
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

        /// <summary>
        /// Checks if the player can cancel recovery based on rhythm performance
        /// </summary>
        private bool CanRhythmCancel()
        {
            if (!rhythmCancelEnabled) return true; // Default behavior
            
            // Only allow cancel on Good or Perfect hits
            return lastRhythmTier == RhythmBonusJudge.RhythmTier.Perfect || 
                   lastRhythmTier == RhythmBonusJudge.RhythmTier.Good;
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
                if (Time.time - lastAttackTime > comboResetDelay)
                    currentComboStep = 0;

                attackData = comboAttacks[Mathf.Clamp(currentComboStep, 0, comboAttacks.Length - 1)];
                currentComboStep++;
                lastAttackTime = Time.time;
            }

            // EVALUATE RHYTHM TIMING
            EvaluateRhythmTiming();

            // PERFORM ATTACK
            Attack(attackData);

            // SET COOLDOWN (modified by rhythm)
            float modifiedCooldown = attackData.cooldown * currentRecoveryModifier;
            nextAttackTime = Time.time + modifiedCooldown;
        }

        /// <summary>
        /// Evaluates the current attack timing against the rhythm and sets modifiers
        /// </summary>
        private void EvaluateRhythmTiming()
        {
            if (rhythmJudge == null)
            {
                lastRhythmTier = RhythmBonusJudge.RhythmTier.Miss;
                currentRecoveryModifier = 1f;
                return;
            }

            // Get rhythm evaluation
            var (tier, multiplier) = rhythmJudge.EvaluateNow();
            lastRhythmTier = tier;

            // Set recovery modifier based on timing
            switch (tier)
            {
                case RhythmBonusJudge.RhythmTier.Perfect:
                    currentRecoveryModifier = perfectRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=cyan>PERFECT!</color> Recovery: {perfectRecoveryMult * 100}%");
                    break;
                    
                case RhythmBonusJudge.RhythmTier.Good:
                    currentRecoveryModifier = goodRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=green>Good!</color> Recovery: {goodRecoveryMult * 100}%");
                    break;
                    
                case RhythmBonusJudge.RhythmTier.Miss:
                    currentRecoveryModifier = missRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=red>Miss...</color> Recovery: {missRecoveryMult * 100}%");
                    break;
            }
        }

        private bool TryUseFinisher()
        {
            AttackData chosenFinisher = null;

            if (currentHitCount >= 9) chosenFinisher = finisher9Hit;
            else if (currentHitCount >= 6) chosenFinisher = finisher6Hit;
            else if (currentHitCount >= 3) chosenFinisher = finisher3Hit;

            if (chosenFinisher != null)
            {
                // Evaluate rhythm for finisher too
                EvaluateRhythmTiming();
                Attack(chosenFinisher);
                currentHitCount = 0;
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
            playerController.SetAttackLock(true);

            Vector3 facing = playerController.FacingDirection;
            Quaternion facingRot = Quaternion.LookRotation(facing, Vector3.up)
                                   * Quaternion.Euler(attackData.rotationOffset);
            Vector3 spawnPos = transform.position + facingRot * attackData.hitboxOffset;

            // Start attack phases with rhythm-modified timing
            StartCoroutine(HandleAttackPhases(attackData, spawnPos, facingRot, facing));
        }

        private IEnumerator HandleAttackPhases(AttackData attackData, Vector3 spawnPos, Quaternion rot, Vector3 facing)
        {
            // STARTUP (not affected by rhythm)
            yield return new WaitForSeconds(attackData.startupTime);

            GameObject hb = null;
            GameObject hbProj = null;

            // ACTIVE
            if (attackData.projectilePrefab == null)
            {
                hb = Instantiate(hitBoxPrefab, spawnPos, rot, transform);
                if (hb.TryGetComponent<Hitbox>(out Hitbox hbComp))
                {
                    hbComp.Initialize(attackData, this.gameObject);
                    hbComp.SetOwnerCombat(this);
                }
            }
            else
            {
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

            // Keep hitbox alive for activeTime (not affected by rhythm)
            yield return new WaitForSeconds(attackData.activeTime);

            if (hb != null) Destroy(hb);
            if (hbProj != null) Destroy(hbProj);

            // RECOVERY - RHYTHM-MODIFIED TIMING
            isInRecovery = true;
            float modifiedRecovery = attackData.recoveryTime * currentRecoveryModifier;
            yield return new WaitForSeconds(modifiedRecovery);

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

            if (showRhythmFeedback)
            {
                Debug.Log($"Hit Count: {currentHitCount}");

                if (currentHitCount == 3)
                    Debug.Log("<color=yellow>3-Hit Finisher unlocked!</color>");
                else if (currentHitCount == 6)
                    Debug.Log("<color=orange>6-Hit Finisher unlocked!</color>");
                else if (currentHitCount == 9)
                    Debug.Log("<color=purple>9-Hit Finisher unlocked!</color>");

                // TODO: ONLY FOR TESTING - remove when finished.
                m_DebugUI.SetDebugBeatStreak(currentHitCount.ToString());
                if (currentHitCount >= 9) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 9");
                else if (currentHitCount >= 6) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 6");
                else if (currentHitCount >= 3) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 3");
                else m_DebugUI.SetDebugSpecialMoveUnlock("None");
            }
        }
        
        //
        // ========================= DEBUG FUNCTIONS =========================
        //
        private void UpdateDebugUi()
        {
            ;
        }
    }
}
