
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

            // Resize prefab to match AttackData
            transform.localScale = attackData.hitboxSize;

            
            // Destroy hitbox after attack duration
            Destroy(gameObject, attackData.TotalDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == owner) return; // Ignore self-hits

                // Deal damage
                if (other.TryGetComponent<EnemyHP>(out EnemyHP health))
                {
                    health.TakeDamage(attackData.damage);
                    ownerCombat?.RegisterHit();
                }

                if (other.TryGetComponent<Hitstun>(out Hitstun hitstun))
            {
                // Apply the hitstun
                hitstun.ApplyHitstun(0.5f); // or replace 2f with attackData.hitstunDuration if you add that
                Debug.Log("Hitstun applied to " + other.name);
            }
            else
            {
                Debug.Log("No Hitstun component found on " + other.name);
            }


            if (owner.TryGetComponent<PlayerBuffs>(out PlayerBuffs buffs))
            {
                    // If this is the 9-hit finisher, apply the buff
                    if (attackData.grantsLifestealBuff)
                    {
                        buffs.ApplyLifesteal(attackData.lifestealPercent, attackData.lifestealDuration);
                        Debug.Log("Finisher applied lifesteal buff!");
                    }

                    // Heal from damage if lifesteal buff is active
                    if (buffs.IsLifestealActive)
                    {
                        buffs.HealFromDamage(attackData.damage);
                        Debug.Log("Healing applied from lifesteal buff!");
                    }
            }
            

         
        
        }
        
        private PlayerCombat ownerCombat;

        public void SetOwnerCombat(PlayerCombat combat)
        {
            ownerCombat = combat;
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
