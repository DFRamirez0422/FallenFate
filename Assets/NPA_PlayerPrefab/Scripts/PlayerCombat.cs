using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform attackSpawnPoint; // Where hitbox spawns
        [SerializeField] private GameObject hitBoxPrefab;    // Hitbox prefab
        [SerializeField] private AttackData defaultAttack;   // Attack stats
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0; // Basic attack button

        private bool isAttacking = false;
        
        // Update is called once per frame
        void Update()
        {
            HandleAttackInput();
        }

        private void HandleAttackInput()
        {
           if (Input.GetButtonDown(attackKey) && !isAttacking)
           {
               Attack(defaultAttack);
           }
        }
        
        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null || attackSpawnPoint == null) return;
            
            isAttacking = true;

            // Spawn hitbox
            GameObject hb = Instantiate(hitBoxPrefab, attackSpawnPoint.position, attackSpawnPoint.rotation);
            Hitbox hbComp = hb.GetComponent<Hitbox>();
            if (hbComp != null)
            {
                hbComp.Initialize(attackData, this.gameObject);
            }
            
            Invoke(nameof(ResetAttack), attackData.attackDuration);
        }

        private void ResetAttack()
        {
            isAttacking = false;
        }
    }
}