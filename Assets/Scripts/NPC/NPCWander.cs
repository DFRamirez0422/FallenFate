using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Basic wondering state movement for the non playable characters.
/// </summary>
public class NPCWander : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Header("Wander Area")]
    [SerializeField] private Vector2 m_WanderArea = new Vector2(5f, 5f);
    [SerializeField] private Vector2 m_StartPosition;

    [Header("Movement Control")]
    [Tooltip("Wandering movement speed in meters per second.")]
    [SerializeField] private float m_WanderSpeed = 2.0f;
    [Tooltip("Amount of time to pause between movement once reached the end of the path.")]
    [SerializeField] private float m_WaitDuration = 1.5f;


    // ===== PUBLIC FIELDS ===== //

    /// <summary>
    /// Exposed variable to retrieve the current movement direction.
    /// </summary>
    public Vector2 MoveDirection => ((Vector3)m_Target - transform.position).normalized;


    // ===== PRIVATE FIELDS ===== //

    private Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_Animator;
    private Vector2 m_Target;
    private bool m_IsPaused = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<PlayerAnimator>();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitAndPickNewTarget());
    }

    private void FixedUpdate()
    {
        if (m_IsPaused)
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, m_Target) < 0.1f)
        {
            StartCoroutine(WaitAndPickNewTarget());
        }

        Vector2 direction = MoveDirection * m_WanderSpeed;
        m_Rigidbody.AddForce(direction - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);
        m_Animator.SetCurrentDirection(direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector3)m_StartPosition, (Vector3)m_WanderArea);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(WaitAndPickNewTarget());
    }

    /// <summary>
    /// Waits an amount of time once the target is reached to choose a new target.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitAndPickNewTarget()
    {
        m_IsPaused = true;
        yield return new WaitForSeconds(m_WaitDuration);

        m_Target = GetRandomTarget();
        m_IsPaused = false;
    }

    /// <summary>
    /// Chooses a new target position in which the character should move towards based on the rectangle bounds.
    /// </summary>
    /// <returns>Target position to wonder towards.</returns>
    private Vector2 GetRandomTarget()
    {
        float half_width = m_WanderArea.x * 0.5f;
        float half_height = m_WanderArea.y * 0.5f;
        int edge_idx = Random.Range(0, 4);

        return edge_idx switch
        {
            // Left Edge
            0 => new Vector2(m_StartPosition.x - half_width, Random.Range(m_StartPosition.y - half_height, m_StartPosition.y + half_height)),
            
            // Right Edge
            1 => new Vector2(m_StartPosition.x + half_width, Random.Range(m_StartPosition.y - half_height, m_StartPosition.y + half_height)),
            
            // Bottom Edge
            2 => new Vector2(Random.Range(m_StartPosition.x - half_width, m_StartPosition.x + half_width), m_StartPosition.y - half_height),
            
            // Top Edge
            _ => new Vector2(Random.Range(m_StartPosition.x - half_width, m_StartPosition.x + half_width), m_StartPosition.y + half_height),
        };
    }
}
