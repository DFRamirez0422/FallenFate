using NPA_RhythmBonusPrefabs;
using System.Collections;
using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    /// <summary>
    /// Rhythm-enhanced combat system that rewards on-beat attacks with faster cancels
    /// and punishes off-beat attacks with sluggish recovery
    /// 
    /// EDITED BY: Jose E.
    /// This copy is to be used for my own testing purposes of animation controller. Unfortunately,
    /// the original class has no public fields that I can use, so a simple decorator pattern is out of the
    /// window.
    /// </summary>
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerCombatRhythmAnimatorAndDebug : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject hitBoxPrefab;
        [SerializeField] private AttackData defaultAttack;
        [SerializeField] private PlayerControllerAnimatorAndDebug playerController; //<--- CHANGED BY: Jose E.
        [SerializeField] private AttackData dashAttackData;

        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

        [Header("Combo Settings")]
        [SerializeField] private AttackData[] comboAttacks;
        [SerializeField] private float attackCooldown = 0.25f;
        [SerializeField] private float comboResetDelay = 1f;

        [Header("Finisher Input Buttons")]
        [SerializeField] private KeyCode finisher3Key = KeyCode.Alpha1;
        [SerializeField] private KeyCode finisher6Key = KeyCode.Alpha2;
        [SerializeField] private KeyCode finisher9Key = KeyCode.Alpha3;

        [SerializeField] private BeatComboCounter rhythmCombo;

        [SerializeField] private AttackData finisher3Hit;
        [SerializeField] private AttackData finisher6Hit;
        [SerializeField] private AttackData finisher9Hit;

        private int currentHitCount = 0;
        private int currentComboStep = 0;
        private float nextAttackTime = 0f;
        private float lastAttackTime = 0f;
        private bool isAttacking = false;

        private int lastLRSign = 1;

        // vvvvv Added by Jose E. from original file. vvvvv //

        /// <summary>
        /// Expoed public variable to tell exactly what attack the player just used.
        /// </summary>
        public AttackData CurrentAttack => m_LastUsedAttack;

        /// <summary>
        /// Exposed public variable to tell whether or not the player is currently attacking.
        /// </summary>
        public bool IsAttacking => isAttacking && m_LastUsedAttack;

        // ^^^^^ Added by Jose E. from original file. ^^^^^ //

        //---- Added by Jose E. from original file. -----//

        [Header("Debug (ONLY FOR TESTING)")]
        [Tooltip("Text object to display the current player state.")]
        [SerializeField] private PlayerDebugUI m_DebugUI;
        [Tooltip("Animator controller used for testing. NOT YET READY!")]
        [SerializeField] private PlayerAnimator m_Animator;
        private AttackData m_LastUsedAttack;

        private Vector3 GetTwoDirFacing()
        {
            Vector3 f = playerController != null ? playerController.FacingDirection : Vector3.right;
            if (Mathf.Abs(f.x) >= 0.01f)
                lastLRSign = f.x >= 0f ? 1 : -1;
            return lastLRSign == 1 ? Vector3.right : Vector3.left;
        }

        private void UpdateFinisherUnlocks()
        {
            int currentCombo = rhythmCombo.GetCurrentCombo();
            if (currentCombo >= 3) finisher3Unlocked = true;
            if (currentCombo >= 6) finisher6Unlocked = true;
            if (currentCombo >= 9) finisher9Unlocked = true;
        }

        public bool finisher3Unlocked = false;
        public bool finisher6Unlocked = false;
        public bool finisher9Unlocked = false;

        void Update()
        {
            UpdateFinisherUnlocks();
            HandleAttackInput();
            HandleFinisherInput();

            // CHANGED BY: Jose E.
            //UpdateAnimation();
            UpdateDebugUi(); // <--- TODO: remove when debugging code is finished
        }

        private void HandleAttackInput()
        {
            if (Input.GetKeyDown(attackKey) && !isAttacking && Time.time >= nextAttackTime)
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

                Attack(attackData);
                nextAttackTime = Time.time + attackData.cooldown;
            }
        }

        private void HandleFinisherInput()
        {
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                if (finisher3Unlocked && Input.GetKeyDown(finisher3Key))
                {
                    Attack(finisher3Hit);
                    finisher3Unlocked = false;
                    rhythmCombo.ResetCombo();
                }
                if (finisher6Unlocked && Input.GetKeyDown(finisher6Key))
                {
                    Attack(finisher6Hit);
                    finisher6Unlocked = false;
                    rhythmCombo.ResetCombo();
                }
                if (finisher9Unlocked && Input.GetKeyDown(finisher9Key))
                {
                    Attack(finisher9Hit);
                    finisher9Unlocked = false;
                    rhythmCombo.ResetCombo();
                }
            }
        }

        private void Attack(AttackData attackData)
        {
            m_LastUsedAttack = attackData; // CHANGED: Jose E.

            if (attackData == null || hitBoxPrefab == null) return;

            var tier = rhythmCombo.EvaluateBeat();
            isAttacking = true;
            playerController.SetAttackLock(true);

            Vector3 lrFacing = GetTwoDirFacing();
            Quaternion facingRot = Quaternion.LookRotation(lrFacing, Vector3.up)
                                   * Quaternion.Euler(attackData.rotationOffset);
            Vector3 spawnPos = transform.position + facingRot * attackData.hitboxOffset;
            playerController.SetAttackSpeed(attackData.forwardOffset);

            StartCoroutine(HandleAttackPhases(attackData, spawnPos, facingRot, lrFacing));
        }

        private IEnumerator HandleAttackPhases(AttackData attackData, Vector3 spawnPos, Quaternion rot, Vector3 lrFacing)
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
                    //hbComp.SetOwnerCombat(this); // CHANGED: Jose E.; does not compile
                }
            }
            else
            {
                hbProj = Instantiate(attackData.projectilePrefab, spawnPos, rot);

                if (hbProj.TryGetComponent<Hitbox>(out Hitbox hbProjComp))
                {
                    hbProjComp.Initialize(attackData, this.gameObject);
                    //hbProjComp.SetOwnerCombat(this); // CHANGED: Jose E.; does not compile
                }

                if (hbProj.TryGetComponent<ProjectileMover>(out ProjectileMover mover))
                {
                    mover.direction = lrFacing;
                    mover.speed = attackData.projectileSpeed;
                    hbProj.transform.rotation = Quaternion.LookRotation(lrFacing, Vector3.up);
                }
            }

            yield return new WaitForSeconds(attackData.activeTime);

            if (hb != null) Destroy(hb);
            if (hbProj != null) Destroy(hbProj);

            yield return new WaitForSeconds(attackData.recoveryTime);

            isAttacking = false;
            playerController.SetAttackLock(false);

            nextAttackTime = Time.time + attackData.cooldown;

            if (currentComboStep >= comboAttacks.Length)
                currentComboStep = 0;
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

        private void ResetAttack()
        {
            isAttacking = false;
            playerController.SetAttackLock(false); // Unlock movement after attack ends

            nextAttackTime = Time.time + attackCooldown;

            // Reset combo if finished
            if (currentComboStep >= comboAttacks.Length)
                currentComboStep = 0;
        }
        
        //
        // ========================= DEBUG FUNCTIONS =========================
        //
        
        /// <summary>
        /// Updates the current animation state by sending messages to the animator controller.
        /// 
        /// Side note: I would like to split this to a different script or integrate this in a
        /// better way but there's not many public fields in either PlayerController or PlayerCombat
        /// that are usable for my purpose.
        /// </summary>
        private void UpdateAnimation()
        {
            if (isAttacking && m_LastUsedAttack)
            {
                m_Animator.SetPlayerIsAttacking(m_LastUsedAttack);
            }
        }

        private void UpdateDebugUi()
        {
            // TODO: ONLY FOR TESTING - remove when finished.
            int current_combo = rhythmCombo.GetCurrentCombo();
            m_DebugUI.SetDebugBeatStreak(current_combo.ToString());
            m_DebugUI.SetDebugComboStep(currentComboStep.ToString());
            if (current_combo >= 9) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 9");
            else if (current_combo >= 6) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 6");
            else if (current_combo >= 3) m_DebugUI.SetDebugSpecialMoveUnlock("Finisher 3");
            else m_DebugUI.SetDebugSpecialMoveUnlock("None");
        }
    }
}

