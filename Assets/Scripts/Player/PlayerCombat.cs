using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Amount of damage to the enemies upon attacking.")] [SerializeField]
    private int m_Damage = 1;

    [Tooltip("Attack cooldown in seconds.")] [SerializeField]
    private float m_Cooldown = 2.0f;

    [SerializeField] private Transform m_AttackPoint;

    [Tooltip("Range of the weapon for attack checking, in meters.")] [SerializeField]
    private float m_WeaponRange;

    [Tooltip("Amount of force for the enemy knock back.")] [SerializeField]
    private float m_KnockBackForce;

    [Tooltip("Duration of knockback velocity in seconds.")] [SerializeField]
    private float m_KnockbackTime = 0.15f;

    [Tooltip("Amount of time to stun the enemy after knockback.")] [SerializeField]
    private float m_StunTime;

    [Tooltip("Enemy collision mask.")] [SerializeField]
    private LayerMask m_EnemyLayer;

    [Tooltip("Enable input handling in Update. If false, call Attack() externally.")] [SerializeField]
    private bool m_HandleInput = false;

    [Tooltip("Show attack range gizmo in editor.")] [SerializeField]
    private bool m_ShowGizmo = true;

    [Header("Attack Point Follow")] [SerializeField]
    private float m_AttackPointDistance = 0.5f; // how far in front of the player

    [SerializeField] private Vector2 m_AttackPointOffset = Vector2.zero;

    [Header("Attack Animation State Names")] [SerializeField]
    private PlayerAnimator m_PlayerAnimator;

    [SerializeField] private string m_AttackUpState = "AttackUp";
    [SerializeField] private string m_AttackDownState = "AttackDown";
    [SerializeField] private string m_AttackLeftState = "AttackLeft";
    [SerializeField] private string m_AttackRightState = "AttackRight";
    [SerializeField] private AudioSource m_PlayerAudio;
    [SerializeField] private AudioClip m_AttackSwing;



// ===== PRIVATE FIELDS ===== //
    private Animator m_Animator;
    private float m_Timer;

    void Update()
    {
        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
        }
        
        
        // Optional input handling
        if (m_HandleInput && Input.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();

    
        if (m_PlayerAnimator == null)
            m_PlayerAnimator = GetComponent<PlayerAnimator>();
    }

    public void Attack()
    {
        if (m_Timer <= 0)
        {
            
            
            if (m_PlayerAnimator != null)
            {
                Vector2 dir = m_PlayerAnimator.LastMovedDirection;

                
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    m_Animator.Play(dir.x >= 0 ? m_AttackRightState : m_AttackLeftState);
                }
                else
                {
                    m_Animator.Play(dir.y >= 0 ? m_AttackUpState : m_AttackDownState);
                }
            }

            m_Animator.SetBool("IsAttacking", true);
            // Play Attack audio
            if (m_PlayerAudio != null)
            {
              m_PlayerAudio.PlayOneShot(m_AttackSwing);  
            }
            
            m_Timer = m_Cooldown;
        }
    }
    
   


    public void DealDamage()
    {
        if (m_AttackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_WeaponRange, m_EnemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            // Check for health component
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health == null) continue;

            // Deal damage
            health.ChangeHealth(-m_Damage);

            // Apply knockback
            EnemyKnockback knockback = enemy.GetComponent<EnemyKnockback>();
            if (knockback != null)
            {
                knockback.Knockback(transform, m_KnockBackForce, m_KnockbackTime, m_StunTime);
            }

            // // Commented out to allow multiple enemies to be hit by one attack - David G
            // // Only hit one enemy per attack
            // break;
        }
    }

    public void FinishAttacking()
    {
        m_Animator.SetBool("IsAttacking", false);
    }
    

    private void OnDrawGizmosSelected()
    {
        if (!m_ShowGizmo || m_AttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_WeaponRange);
    }
}
