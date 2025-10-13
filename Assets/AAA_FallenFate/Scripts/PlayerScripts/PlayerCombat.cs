using NPA_RhythmBonusPrefabs;
using System.Collections;
using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        // [SerializeField] private Transform attackSpawnPoint; // Where hitbox spawns
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

        [Header("Finisher Input Buttons")]
        [SerializeField] private KeyCode finisher3Key = KeyCode.Alpha1;
        [SerializeField] private KeyCode finisher6Key = KeyCode.Alpha2;
        [SerializeField] private KeyCode finisher9Key = KeyCode.Alpha3;

        // --- Combo Finisher System ---

        [SerializeField] private AttackData finisher3Hit;
        [SerializeField] private AttackData finisher6Hit;
        [SerializeField] private AttackData finisher9Hit;

        private int currentHitCount = 0; // Tracks consecutive successful hits


        private int currentComboStep = 0; 
        private float nextAttackTime = 0f;
        private float lastAttackTime = 0f;

        private bool isAttacking = false; // Prevents attack spamming

        [SerializeField] private BeatComboCounter rhythmCombo;

        // Finisher unlock states
        public bool finisher3Unlocked = false;
        public bool finisher6Unlocked = false;
        public bool finisher9Unlocked = false;

        private void UpdateFinisherUnlocks()
        {
            int currentCombo = rhythmCombo.GetCurrentCombo();
            
            if (currentCombo >= 3) finisher3Unlocked = true;
            if (currentCombo >= 6) finisher6Unlocked = true;
            if (currentCombo >= 9) finisher9Unlocked = true;
        }


        void Update()
        {
            UpdateFinisherUnlocks();
            HandleAttackInput();
            HandleFinisherInput();
        }// Check for player input each frame
        

        private void HandleAttackInput()
        {
            // Check if player pressed attack, is not already attacking, 
            // and cooldown has passed (per-attack)
            if (Input.GetKeyDown(attackKey) && !isAttacking && Time.time >= nextAttackTime)
            {
                AttackData attackData; // This will hold the attack we perform

                //  DASH ATTACK LOGIC 
                if (playerController.DashAttackWindowActive)
                {
                    attackData = dashAttackData;            // Use dash attack data
                    playerController.ConsumeDashAttack();   // Consume the dash attack "window"
                }
                else
                {
                    //  COMBO ATTACK LOGIC 
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

                //  PERFORM ATTACK 
                Attack(attackData); // Call your attack function

                //  SET COOLDOWN 
                // Each attack can have its own cooldown to control pacing
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
                }
                if (finisher6Unlocked && Input.GetKeyDown(finisher6Key))
                {
                    Attack(finisher6Hit);
                    finisher6Unlocked = false;
                }
                if (finisher9Unlocked && Input.GetKeyDown(finisher9Key))
                {
                    Attack(finisher9Hit);
                    finisher9Unlocked = false;
                }
            }

        }

        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null) return;

            var tier = rhythmCombo.EvaluateBeat();
            isAttacking = true;
            playerController.SetAttackLock(true); // Freeze or slow movement

            Vector3 facing = playerController.FacingDirection;
            Quaternion facingRot = Quaternion.LookRotation(facing, Vector3.up)
                                   * Quaternion.Euler(attackData.rotationOffset);
            Vector3 spawnPos = transform.position + facingRot * attackData.hitboxOffset;
            playerController.SetAttackSpeed(attackData.forwardOffset);

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

            // RECOVERY
            yield return new WaitForSeconds(attackData.recoveryTime);

            // Unlock player movement
            isAttacking = false;
            playerController.SetAttackLock(false);

            // Set cooldown for next attack
            nextAttackTime = Time.time + attackData.cooldown;
            // reset Combo
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
    }
}