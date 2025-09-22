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
           
          
            if (Input.GetKeyDown(attackKey) && !isAttacking && Time.time >= nextAttackTime)
            {
                    // Reset combo if too slow
                if (Time.time - lastAttackTime > comboResetDelay)
                    currentComboStep = 0;

                    // Pick which attack in the chain
                AttackData attackData = comboAttacks[Mathf.Clamp(currentComboStep, 0, comboAttacks.Length - 1)];

                    // Advance step
                currentComboStep++;
                lastAttackTime = Time.time;

                Attack(attackData);
            }
           

            // Only trigger an attack if not already mid-attack
           if (Input.GetKeyDown(attackKey) && !isAttacking)
           {
               // Use DashAttack if within dash window, else normal attack
               if (playerController.DashAttackWindowActive)
               {
                   Attack(dashAttackData);
                   playerController.ConsumeDashAttack();
               }
               else
               {
                   Attack(defaultAttack);
               }
           }
        }
        
        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null) return;
            
            isAttacking = true;
            playerController.SetAttackLock(true); // Freeze movement during attack

            // Build facing rotation (direction from controller + attack-specific offset)
            Vector3 facing = playerController.FacingDirection;
            Quaternion facingRot = Quaternion.LookRotation(facing, Vector3.up)
                                   * Quaternion.Euler(attackData.rotationOffset);

            // Spawn position relative to player
            Vector3 spawnPos = transform.position + facingRot * attackData.hitboxOffset;

            // Spawn hitbox under player
            GameObject hb = Instantiate(hitBoxPrefab, spawnPos, facingRot, transform);

            // Initialize with attack parameters
            Hitbox hbComp = hb.GetComponent<Hitbox>();
            if (hbComp != null)
            {
                hbComp.Initialize(attackData, this.gameObject);
            }
            // End attack after its duration
            Invoke(nameof(ResetAttack), attackData.attackDuration);
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