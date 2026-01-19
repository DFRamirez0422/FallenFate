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

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_WeaponRange, m_PlayerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-m_Damage);
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, m_KnockBackForce, m_StunTime);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_WeaponRange);
    }
}
