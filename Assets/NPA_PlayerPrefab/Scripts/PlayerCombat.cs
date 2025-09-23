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

        private int currentComboStep = 0; 
        private float nextAttackTime = 0f;
        private float lastAttackTime = 0f;

        private bool isAttacking = false; // Prevents attack spamming
        
        void Update()
        {
            HandleAttackInput(); // Check for player input each frame
        }

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
            StartCoroutine(HandleAttackPhases(attackData, spawnPos, facingRot));
        }
        private System.Collections.IEnumerator HandleAttackPhases(AttackData attackData, Vector3 spawnPos, Quaternion rot)
        {
            // --- STARTUP ---
            yield return new WaitForSeconds(attackData.startupTime);

            // --- ACTIVE ---
            GameObject hb = Instantiate(hitBoxPrefab, spawnPos, rot, transform);
            Hitbox hbComp = hb.GetComponent<Hitbox>();
            if (hbComp != null)
                hbComp.Initialize(attackData, this.gameObject);

            yield return new WaitForSeconds(attackData.activeTime);

            // Destroy hitbox after active frames
            if (hb != null)
                Destroy(hb);

            // --- RECOVERY ---
            yield return new WaitForSeconds(attackData.recoveryTime);

            // Unlock movement and reset attack state
            isAttacking = false;
            playerController.SetAttackLock(false);

            // Set cooldown for next attack
            nextAttackTime = Time.time + attackData.cooldown;

            // Reset combo if finished
            if (currentComboStep >= comboAttacks.Length)
                currentComboStep = 0;
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