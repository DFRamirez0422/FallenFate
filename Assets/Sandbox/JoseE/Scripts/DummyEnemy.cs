using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteAnimator))]
public class DummyEnemy : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Chasing speed in meters per second.")]
    [SerializeField] private float m_ChaseSpeed = 3.0f;
    [Tooltip("Distance from the player to stop chasing.")]
    [SerializeField] private float m_StopDistance = 0.75f;

    [Tooltip("Amount of damage to the player upon attacking.")]
    [SerializeField] private int m_Damage = 1;

    // ===== PRIVATE FIELDS ===== //
    private Transform m_Player;
    private Rigidbody2D m_Rigidbody;
    private SpriteAnimator m_Animator;
    private bool m_IsChasing = false;
    private bool m_IsStunned;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        // Chase the player if in range
        if (m_IsChasing && !m_IsStunned)
        {
            Vector2 enemy_to_player = m_Player.position - transform.position;
            float distance = enemy_to_player.magnitude;

            // Stop at a safe distance
            if (distance <= m_StopDistance)
            {
                m_Rigidbody.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction = enemy_to_player.normalized * m_ChaseSpeed;
            m_Rigidbody.AddForce(
                direction - m_Rigidbody.linearVelocity,
                ForceMode2D.Impulse
            );

            m_Animator.SetCurrentDirection(enemy_to_player);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().Hit(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!m_Player) m_Player = collision.gameObject.transform;
            m_IsChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_IsChasing = false;
            m_Rigidbody.linearVelocity = Vector2.zero;
        }
    }
    
    // --- Set stunned state --- //
    public void SetStunned(bool value)
    {
        m_IsStunned = value;
    }
}
