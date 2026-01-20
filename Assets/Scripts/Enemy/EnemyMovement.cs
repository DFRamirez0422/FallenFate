using UnityEditor.EditorTools;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimator))]
public class EnemyMovement : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Chasing speed in meters per second.")]
    [SerializeField] private float m_ChaseSpeed = 3.0f;
    [Tooltip("Attack range in meters.")]
    [SerializeField] private float m_AttackRange = 2.0f;
    [Tooltip("Duration of the cooldown between attacks in seconds.")]
    [SerializeField] private float m_AttackCooldown = 2.0f;
    [Tooltip("The distance within the enemy will SEE the player.")]
    [SerializeField] private float m_PlayerDetectRange = 5.0f;
    [Tooltip("The center point of the enemy's circle of sight.")]
    [SerializeField] private Transform m_DetectionPoint;
    [Tooltip("Player collision mask.")]
    [SerializeField] private LayerMask m_PlayerLayer;


    // ===== PRIVATE FIELDS ===== //
    private Transform m_Player;
    private Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_SpriteAnimator;
    private Animator m_Animator;
    private float m_AttackCooldownTimer;
    private int m_FacingDirection = -1;
    private EnemyState m_EnemyState;
    private bool m_IsStunned;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteAnimator = GetComponent<PlayerAnimator>();
        m_Animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        // Don't process AI if stunned
        if (m_IsStunned) return;

        CheckForPlayer();

        if (m_EnemyState != EnemyState.Knockback)
        {
            if (m_AttackCooldownTimer > 0)
            {
                m_AttackCooldownTimer -= Time.deltaTime;
            }

            if (m_EnemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (m_EnemyState == EnemyState.Attacking)
            {
                m_Rigidbody.linearVelocity = Vector2.zero;
            }
        }
    }

    void Chase()
    {
        // Made redundant due to our use of the SpriteAnimator class.
        // if (m_Player.position.x > transform.position.x && m_FacingDirection == -1 ||
        //     m_Player.position.x < transform.position.x && m_FacingDirection == 1)
        // {
        //     Flip();
        // }

        Vector2 enemy_to_player = m_Player.position - transform.position;
        Vector2 direction = enemy_to_player.normalized * m_ChaseSpeed;
        m_Rigidbody.AddForce(direction - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);
        m_SpriteAnimator.SetCurrentDirection(enemy_to_player);
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(m_DetectionPoint.position, m_PlayerDetectRange, m_PlayerLayer);

        if (hits.Length > 0)
        {
            m_Player =  hits[0].transform;
    
            if (Vector2.Distance(transform.position, m_Player.position) < m_AttackRange && m_AttackCooldownTimer <= 0)
            {
                m_AttackCooldownTimer = m_AttackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (Vector2.Distance(transform.position, m_Player.position) > m_AttackRange && m_EnemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    private void Flip()
    {
        m_FacingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(EnemyState new_state)
    {
        // Exit the current animation.
        if (m_EnemyState == EnemyState.Idle)
        {
            m_Animator.SetBool("IsIdle", false);
        }
        else if (m_EnemyState == EnemyState.Chasing)
        {
            m_Animator.SetBool("IsChasing", false);
        }
        else if (m_EnemyState == EnemyState.Attacking)
        {
            m_Animator.SetBool("IsAttacking", false);
        }

        m_EnemyState = new_state;

        // Enter the new animation.
        if (m_EnemyState == EnemyState.Idle)
        {
            m_Animator.SetBool("IsIdle", true);
        }
        else if (m_EnemyState == EnemyState.Chasing)
        {
            m_Animator.SetBool("IsChasing", true);
        }
        else if (m_EnemyState == EnemyState.Attacking)
        {
            m_Animator.SetBool("IsAttacking", true);
        }
    }

    /// <summary>
    /// Set the stunned state of the enemy. When stunned, the enemy will not process AI or chase the player.
    /// Note: This does not affect velocity when in Knockback state to allow knockback physics to work.
    /// </summary>
    /// <param name="value">True to stun the enemy, false to unstun.</param>
    public void SetStunned(bool value)
    {
        m_IsStunned = value;
        // Only zero velocity if not in knockback state (to allow knockback physics to work)
        if (m_IsStunned && m_EnemyState != EnemyState.Knockback)
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback,
};