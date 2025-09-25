using UnityEngine;
using NPA_Health_Components;

namespace NPA_PlayerPrefab.Scripts
{
    public class Hitbox : MonoBehaviour
    {
        private AttackData attackData;                   // Attack parameters
        private GameObject owner;                        // Who spawned this hitbox
        [SerializeField] private bool showGizmo = false; // Toggle gizmo in editor

        public void Initialize(AttackData data, GameObject ownerObj)
        {
            attackData = data;
            owner = ownerObj;

            // Resize collider to match AttackData
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null) col.size = attackData.hitboxSize;
            
            // Destroy hitbox after attack duration
            Destroy(gameObject, attackData.TotalDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == owner) return; // Ignore self-hits

            // Apply damage if target has Health
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackData.damage);
            }
            if (other.TryGetComponent<Hitstun>(out Hitstun hitstun))
            {
                // maybe will add duration from AttackData
                hitstun.ApplyHitstun(2f); 
            }

        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmo) return;
            
            // Draw collider outline in Scene View
            BoxCollider col = GetComponent<BoxCollider>();
            
            if (col == null) return;
            
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.center, col.size);
        }
    }
}
