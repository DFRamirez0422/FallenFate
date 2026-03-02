using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //

    [Tooltip("Amount of damage to the player upon attacking.")]
    [SerializeField] private int m_Damage = 1;
    [Tooltip("Game object transform in which to check whether the player collided with the attack point.")]
    [SerializeField] private Transform m_AttackPoint;
    [Tooltip("Range of the weapon for attack checking, in meters.")]
    [SerializeField] private float m_WeaponRange;
    [Tooltip("Amount of force for the player knock back.")]
    [SerializeField] private float m_KnockBackForce;
    [Tooltip("Amount of time to stun the player during knockback.")]
    [SerializeField] private float m_StunTime;
    [Tooltip("Player collision mask.")]
    [SerializeField] private LayerMask m_PlayerLayer;

    /// <summary>
    ///  Checks for player collision with the attack point and applies damage and knockback if hit.
    /// </summary>
    public void EnemyAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_WeaponRange, m_PlayerLayer);

        // Only apply damage and knockback to the first player hit, if any. This prevents multiple hits from one attack.
        if (hits.Length > 0)
        {
            var health = hits[0].GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(m_Damage, transform);

            var movement = hits[0].GetComponent<PlayerMovement>();
            if (movement != null)
                movement.Knockback(transform, m_KnockBackForce, m_StunTime);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_WeaponRange);
    }
}
