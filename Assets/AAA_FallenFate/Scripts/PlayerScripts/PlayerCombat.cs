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
        
        void Update()
        {
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
            if (Input.GetKeyDown(finisherKey) && !isAttacking)
            {
                AttackData chosenFinisher = null;

                if (currentHitCount >= 9) chosenFinisher = finisher9Hit;
                else if (currentHitCount >= 6) chosenFinisher = finisher6Hit;
                else if (currentHitCount >= 3) chosenFinisher = finisher3Hit;

                if (chosenFinisher != null)
                {
                    Attack(chosenFinisher);

                    // Reset hit counter after using finisher
                    currentHitCount = 0;
                }
            }
        }
        
        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null) return;

            isAttacking = true;
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