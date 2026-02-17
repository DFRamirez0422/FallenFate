using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Tooltip("Amount of damage to the enemies upon attacking.")]
    [SerializeField] private int m_Damage = 1;
    [Tooltip("Attack cooldown in seconds.")]
    [SerializeField] private float m_Cooldown = 2.0f;
    [SerializeField] private Transform m_AttackPoint;
    [Tooltip("Range of the weapon for attack checking, in meters.")]
    [SerializeField] private float m_WeaponRange;
    [Tooltip("Amount of force for the enemy knock back.")]
    [SerializeField] private float m_KnockBackForce;
    [Tooltip("Duration of knockback velocity in seconds.")]
    [SerializeField] private float m_KnockbackTime = 0.15f;
    [Tooltip("Amount of time to stun the enemy after knockback.")]
    [SerializeField] private float m_StunTime;
    [Tooltip("Enemy collision mask.")]
    [SerializeField] private LayerMask m_EnemyLayer;
    [Tooltip("Enable input handling in Update. If false, call Attack() externally.")]
    [SerializeField] private bool m_HandleInput = false;
    [Tooltip("Show attack range gizmo in editor.")]
    [SerializeField] private bool m_ShowGizmo = true;

    [Header("Attack Animation State Names")]
    [SerializeField] private PlayerAnimator m_PlayerAnimator;
    [SerializeField] private string m_AttackUpState = "AttackUp";
    [SerializeField] private string m_AttackDownState = "AttackDown";
    [SerializeField] private string m_AttackLeftState = "AttackLeft";
    [SerializeField] private string m_AttackRightState = "AttackRight";
    [Header("Sound (optional - prefers PlayerSound when present)")]
    [SerializeField] private AudioSource m_PlayerAudio;
    [SerializeField] private AudioClip m_AttackSwingClip;
    [SerializeField] private AudioClip m_HurtClip;

    private Animator m_Animator;
    private PlayerSound m_PlayerSound;
    private float m_Timer;

    private void Update()
    {
        if (m_Timer > 0)
            m_Timer -= Time.deltaTime;

        if (m_HandleInput && Input.GetButtonDown("Attack"))
            Attack();
    }

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayerSound = GetComponent<PlayerSound>();

        if (m_PlayerAnimator == null)
            m_PlayerAnimator = GetComponent<PlayerAnimator>();
    }

    public void Attack()
    {
        if (m_Timer > 0) return;

        if (m_PlayerAnimator != null)
        {
            Vector2 dir = m_PlayerAnimator.LastMovedDirection;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                m_Animator.Play(dir.x >= 0 ? m_AttackRightState : m_AttackLeftState);
            else
                m_Animator.Play(dir.y >= 0 ? m_AttackUpState : m_AttackDownState);
        }

        m_Animator.SetBool("IsAttacking", true);
        if (m_PlayerAnimator != null)
            m_PlayerAnimator.StartAttack();
        if (m_PlayerSound != null)
            m_PlayerSound.PlayAttack();
        else if (m_PlayerAudio != null && m_AttackSwingClip != null)
            m_PlayerAudio.PlayOneShot(m_AttackSwingClip);
        m_Timer = m_Cooldown;
    }

    public void DealDamage()
    {
        if (m_AttackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_WeaponRange, m_EnemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health == null) continue;

            health.ChangeHealth(-m_Damage);

            EnemyKnockback knockback = enemy.GetComponent<EnemyKnockback>();
            if (knockback != null)
                knockback.Knockback(transform, m_KnockBackForce, m_KnockbackTime, m_StunTime);
        }
    }

    public void FinishAttacking()
    {
        m_Animator.SetBool("IsAttacking", false);
        if (m_PlayerAnimator != null)
            m_PlayerAnimator.Reset();
    }

    public void HitReact()
    {
        if (m_PlayerSound != null)
            m_PlayerSound.PlayDamage();
        else if (m_PlayerAudio != null && m_HurtClip != null)
            m_PlayerAudio.PlayOneShot(m_HurtClip);
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_ShowGizmo || m_AttackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_WeaponRange);
    }
}
