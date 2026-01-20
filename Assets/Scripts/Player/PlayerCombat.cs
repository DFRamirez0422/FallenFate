using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Amount of damage to the eenemies upon attacking.")]
    [SerializeField] private int m_Damage = 1;
    [Tooltip("Attack cooldown in seconds.")]
    [SerializeField] private float m_Cooldown = 2.0f;
    [SerializeField] private Transform m_AttackPoint;
    [Tooltip("Range of the weapon for attack checking, in meters.")]
    [SerializeField] private float m_WeaponRange;
    [Tooltip("Amount of force for the enemy knock back.")]
    [SerializeField] private float m_KnockBackForce;
    [Tooltip("Amount of time to stun the enemy during knockback.")]
    [SerializeField] private float m_StunTime;
    [Tooltip("Enemy collision mask.")]
    [SerializeField] private LayerMask m_EnemyLayer;


    // ===== PRIVATE FIELDS ===== //
    private Animator m_Animator;
    private float m_Timer;

    void Update()
    {
        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
        }
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (m_Timer <= 0)
        {
            m_Animator.SetBool("IsAttacking", true);
            m_Timer = m_Cooldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_WeaponRange, m_EnemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<EnemyHealth>().ChangeHealth(-m_Damage);
            enemies[0].GetComponent<EnemyKnockback>().Knockback(transform, m_KnockBackForce, m_StunTime);
        }
    }

    public void FininshAttacking()
    {
        m_Animator.SetBool("IsAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_WeaponRange);
    }
}
