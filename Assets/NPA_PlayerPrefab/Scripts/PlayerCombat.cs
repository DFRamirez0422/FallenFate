using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform attackSpawnPoint; // Where hitbox spawns
        [SerializeField] private GameObject hitBoxPrefab;    // Hitbox prefab
        [SerializeField] private AttackData defaultAttack;   // Attack stats
        [SerializeField] private PlayerController playerController; //reference direction for spawning hitbox
        [SerializeField] private bool showGizmo = false; // Toggle gizmo in editor
        
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
           if (Input.GetKeyDown(attackKey) && !isAttacking)
           {
               Attack(defaultAttack);
           }
        }
        
        private void Attack(AttackData attackData)
        {
            if (attackData == null || hitBoxPrefab == null || attackSpawnPoint == null) return;
            
            isAttacking = true;

            // Spawn hitbox
            Vector3 facing = playerController.facingDirection;
            Quaternion facingRot = Quaternion.LookRotation(facing, Vector3.up);

            //Apply offset when using AttackData
            Vector3 spawnPos = attackSpawnPoint.position + facingRot * attackData.hitboxOffset;

            GameObject hb = Instantiate(hitBoxPrefab, spawnPos, facingRot);

            Hitbox hbComp = hb.GetComponent<Hitbox>();
            if (hbComp != null)
            {
                hbComp.Initialize(attackData, this.gameObject);
            }
            
            Invoke(nameof(ResetAttack), attackData.attackDuration);
        }
        private void OnDrawGizmos()
        {
            if (!showGizmo) return;
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null) return;
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.center, col.size);
        }
        private void ResetAttack()
        {
            isAttacking = false;
        }
    }
}