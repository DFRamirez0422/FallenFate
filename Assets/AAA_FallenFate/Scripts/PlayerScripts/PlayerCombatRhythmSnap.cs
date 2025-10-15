using System.Collections;
using UnityEngine;
using NPA_RhythmBonusPrefabs;

namespace NPA_PlayerPrefab.Scripts
{
    /// <summary>
    /// Enhanced rhythm combat that "snaps" buffered inputs to the next beat
    /// Prevents death spirals from missed timing
    /// </summary>
    public class PlayerCombatRhythmSnap : MonoBehaviour, IPlayerCombat
    {
        [Header("References")]
        [SerializeField] private GameObject hitBoxPrefab;
        [SerializeField] private AttackData defaultAttack;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AttackData dashAttackData;
        [SerializeField] private RhythmBonusJudge rhythmJudge;
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        
        [Header("Combo Settings")]
        [SerializeField] private AttackData[] comboAttacks;
        [SerializeField] private float attackCooldown = 0.25f;
        [SerializeField] private float comboResetDelay = 1f;
        
        [Header("Rhythm Timing Modifiers")]
        [SerializeField] private float perfectRecoveryMult = 0.5f;
        [SerializeField] private float goodRecoveryMult = 0.75f;
        [SerializeField] private float missRecoveryMult = 1.5f;
        [SerializeField] private bool rhythmCancelEnabled = true;
        [SerializeField] private bool showRhythmFeedback = true;
        
        [Header("Beat Snapping (NEW)")]
        [Tooltip("If enabled, buffered attacks wait for the next beat window")]
        [SerializeField] private bool enableBeatSnapping = true;
        
        [Tooltip("Maximum delay to wait for next beat (seconds)")]
        [SerializeField] private float maxSnapDelay = 0.3f;
        
        [Tooltip("Auto-snap to beats even without buffering")]
        [SerializeField] private bool alwaysSnapToBeats = false;
        
        [Header("Anti-Spam System")]
        [Tooltip("Disable snapping if player is spamming inputs")]
        [SerializeField] private bool preventSnapOnSpam = true;
        
        [Tooltip("Minimum time between inputs to not be considered spam (seconds)")]
        [SerializeField] private float spamThreshold = 0.15f;
        
        [Tooltip("How many rapid inputs trigger spam detection")]
        [SerializeField] private int spamInputCount = 3;
        
        [Tooltip("After spam detected, how long snapping stays disabled (seconds)")]
        [SerializeField] private float spamCooldown = 1.0f;
        
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
        
        private RhythmBonusJudge.RhythmTier lastRhythmTier = RhythmBonusJudge.RhythmTier.Miss;
        private float currentRecoveryModifier = 1f;
        
        private bool attackBuffered = false;
        private bool finisherBuffered = false;
        private float bufferTimeStamp = 0f;
        
        // Beat snapping state
        private bool waitingForBeat = false;
        private Coroutine snapCoroutine = null;
        
        // Anti-spam detection
        private System.Collections.Generic.Queue<float> recentInputTimes = new System.Collections.Generic.Queue<float>();
        private bool isSpamming = false;
        private float spamDetectedTime = -999f; // Track when spam was last detected
        
        void Update()
        {
            HandleAttackInput();
            HandleFinisherInput();
            
            if (!waitingForBeat)
            {
                ProcessBufferedInputs();
            }
        }

        private void HandleAttackInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                // Track input for spam detection
                DetectSpamming();
                
                if (!isAttacking && Time.time >= nextAttackTime)
                {
                    if (alwaysSnapToBeats && enableBeatSnapping && !isSpamming)
                    {
                        TrySnapToNextBeat(() => PerformAttack());
                    }
                    else
                    {
                        PerformAttack();
                    }
                }
                else if (enableAttackCanceling && isInRecovery && CanRhythmCancel())
                {
                    if (showRhythmFeedback)
                        Debug.Log("<color=yellow>→ Recovery Cancel</color>");
                    PerformAttack();
                }
                else if (enableInputBuffering && isAttacking && !attackBuffered)
                {
                    attackBuffered = true;
                    bufferTimeStamp = Time.time;
                    if (showRhythmFeedback)
                        Debug.Log("<color=cyan>→ Input Buffered</color>");
                }
                else if (isAttacking)
                {
                    if (showRhythmFeedback)
                        Debug.Log("<color=gray>→ Busy (already attacking)</color>");
                }
                else if (Time.time < nextAttackTime)
                {
                    if (showRhythmFeedback)
                    {
                        float cooldownRemaining = nextAttackTime - Time.time;
                        Debug.Log($"<color=orange>→ Cooldown ({cooldownRemaining:F2}s)</color>");
                    }
                }
            }
        }

        private void HandleFinisherInput()
        {
            if (Input.GetKeyDown(finisherKey))
            {
                if (!isAttacking && TryUseFinisher())
                {
                    return;
                }
                else if (enableInputBuffering && isAttacking && !finisherBuffered)
                {
                    finisherBuffered = true;
                    bufferTimeStamp = Time.time;
                }
            }
        }

        private void ProcessBufferedInputs()
        {
            if (isAttacking || Time.time - bufferTimeStamp > bufferWindow)
            {
                if (Time.time - bufferTimeStamp > bufferWindow)
                {
                    attackBuffered = false;
                    finisherBuffered = false;
                }
                return;
            }

            // Process finisher first
            if (finisherBuffered)
            {
                finisherBuffered = false;
                if (TryUseFinisher())
                {
                    attackBuffered = false;
                    return;
                }
            }

            // Process attack buffer with optional beat snapping
            if (attackBuffered)
            {
                attackBuffered = false;
                
                if (enableBeatSnapping && !isSpamming)
                {
                    TrySnapToNextBeat(() => PerformAttack());
                }
                else
                {
                    PerformAttack();
                }
            }
        }

        /// <summary>
        /// Detects if player is spamming inputs and disables snapping accordingly
        /// </summary>
        private void DetectSpamming()
        {
            if (!preventSnapOnSpam)
            {
                isSpamming = false;
                return;
            }

            float currentTime = Time.time;
            
            // Check if we're still in spam cooldown from previous spam
            if (currentTime - spamDetectedTime < spamCooldown)
            {
                isSpamming = true;
                if (showRhythmFeedback)
                {
                    float cooldownRemaining = spamCooldown - (currentTime - spamDetectedTime);
                    Debug.Log($"<color=red>⚠️ SPAM COOLDOWN ({cooldownRemaining:F1}s)</color>");
                }
                return; // Still in cooldown, don't check queue
            }
            
            // Add current input time
            recentInputTimes.Enqueue(currentTime);
            
            // Remove old inputs outside the spam window
            while (recentInputTimes.Count > 0 && currentTime - recentInputTimes.Peek() > spamThreshold * spamInputCount)
            {
                recentInputTimes.Dequeue();
            }
            
            // Check if we have enough rapid inputs to be considered spam
            if (recentInputTimes.Count >= spamInputCount)
            {
                // Check if all inputs are within spam threshold
                float firstInputTime = recentInputTimes.Peek();
                float timeSinceFirst = currentTime - firstInputTime;
                
                if (timeSinceFirst <= spamThreshold * (spamInputCount - 1))
                {
                    isSpamming = true;
                    spamDetectedTime = currentTime; // Mark when spam was detected
                    
                    // Show feedback every time
                    if (showRhythmFeedback)
                    {
                        Debug.Log($"<color=red>⚠️ SPAM DETECTED! Cooldown: {spamCooldown}s</color>");
                    }
                }
                else
                {
                    isSpamming = false;
                }
            }
            else
            {
                isSpamming = false;
            }
        }

        /// <summary>
        /// Waits for the next beat window before executing the attack
        /// </summary>
        private void TrySnapToNextBeat(System.Action attackAction)
        {
            if (rhythmJudge == null || waitingForBeat)
            {
                attackAction?.Invoke();
                return;
            }

            // Check if we're already in a good window
            var (tier, _) = rhythmJudge.EvaluateNow();
            if (tier == RhythmBonusJudge.RhythmTier.Perfect || tier == RhythmBonusJudge.RhythmTier.Good)
            {
                attackAction?.Invoke();
                return;
            }

            // Start waiting for next beat
            if (snapCoroutine != null)
            {
                StopCoroutine(snapCoroutine);
            }
            
            snapCoroutine = StartCoroutine(WaitForNextBeat(attackAction));
        }

        private IEnumerator WaitForNextBeat(System.Action attackAction)
        {
            waitingForBeat = true;
            float waitStartTime = Time.time;

            if (showRhythmFeedback)
            {
                Debug.Log("<color=yellow>Waiting for next beat...</color>");
            }

            // Poll until we hit a good window or timeout
            while (Time.time - waitStartTime < maxSnapDelay)
            {
                var (tier, _) = rhythmJudge.EvaluateNow();
                
                if (tier == RhythmBonusJudge.RhythmTier.Perfect || tier == RhythmBonusJudge.RhythmTier.Good)
                {
                    if (showRhythmFeedback)
                    {
                        Debug.Log($"<color=cyan>Snapped to {tier}!</color>");
                    }
                    
                    waitingForBeat = false;
                    attackAction?.Invoke();
                    yield break;
                }

                yield return null;
            }

            // Timeout - attack anyway
            if (showRhythmFeedback)
            {
                Debug.Log("<color=orange>Snap timeout - attacking anyway</color>");
            }
            
            waitingForBeat = false;
            attackAction?.Invoke();
        }

        private bool CanRhythmCancel()
        {
            if (!rhythmCancelEnabled) return true;
            return lastRhythmTier == RhythmBonusJudge.RhythmTier.Perfect || 
                   lastRhythmTier == RhythmBonusJudge.RhythmTier.Good;
        }

        private void PerformAttack()
        {
            AttackData attackData;

            if (playerController.DashAttackWindowActive)
            {
                attackData = dashAttackData;
                playerController.ConsumeDashAttack();
            }
            else
            {
                if (Time.time - lastAttackTime > comboResetDelay)
                    currentComboStep = 0;

                attackData = comboAttacks[Mathf.Clamp(currentComboStep, 0, comboAttacks.Length - 1)];
                currentComboStep++;
                lastAttackTime = Time.time;
            }

            EvaluateRhythmTiming();
            Attack(attackData);

            float modifiedCooldown = attackData.cooldown * currentRecoveryModifier;
            nextAttackTime = Time.time + modifiedCooldown;
        }

        private void EvaluateRhythmTiming()
        {
            if (rhythmJudge == null)
            {
                lastRhythmTier = RhythmBonusJudge.RhythmTier.Miss;
                currentRecoveryModifier = 1f;
                if (showRhythmFeedback)
                    Debug.Log("<color=gray>⚔️ ATTACK - No rhythm judge</color>");
                return;
            }

            var (tier, multiplier) = rhythmJudge.EvaluateNow();
            lastRhythmTier = tier;

            // Build feedback message with spam status
            string spamStatus = isSpamming ? " [SPAM]" : "";
            string snapStatus = enableBeatSnapping && !isSpamming ? " [SNAP ON]" : "";

            switch (tier)
            {
                case RhythmBonusJudge.RhythmTier.Perfect:
                    currentRecoveryModifier = perfectRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=cyan>⚔️ PERFECT!</color> Recovery: {perfectRecoveryMult * 100}%{spamStatus}{snapStatus}");
                    break;
                    
                case RhythmBonusJudge.RhythmTier.Good:
                    currentRecoveryModifier = goodRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=green>⚔️ Good!</color> Recovery: {goodRecoveryMult * 100}%{spamStatus}{snapStatus}");
                    break;
                    
                case RhythmBonusJudge.RhythmTier.Miss:
                    currentRecoveryModifier = missRecoveryMult;
                    if (showRhythmFeedback)
                        Debug.Log($"<color=red>⚔️ Miss...</color> Recovery: {missRecoveryMult * 100}%{spamStatus}{snapStatus}");
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

            StartCoroutine(HandleAttackPhases(attackData, spawnPos, facingRot, facing));
        }

        private IEnumerator HandleAttackPhases(AttackData attackData, Vector3 spawnPos, Quaternion rot, Vector3 facing)
        {
            yield return new WaitForSeconds(attackData.startupTime);

            GameObject hb = null;
            GameObject hbProj = null;

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

            yield return new WaitForSeconds(attackData.activeTime);

            if (hb != null) Destroy(hb);
            if (hbProj != null) Destroy(hbProj);

            isInRecovery = true;
            float modifiedRecovery = attackData.recoveryTime * currentRecoveryModifier;
            yield return new WaitForSeconds(modifiedRecovery);

            if (isInRecovery)
            {
                isAttacking = false;
                isInRecovery = false;
                playerController.SetAttackLock(false);

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
            }
        }
    }
}
