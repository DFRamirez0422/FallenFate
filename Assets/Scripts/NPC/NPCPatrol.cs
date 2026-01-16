using UnityEngine;
using System.Collections;
using Unity.Burst.Intrinsics;

/// <summary>
/// Basic patrolling state movement for the non playable characters.
/// </summary>
public class NPCPatrol : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("List of game objects to act as control points for the patrolling behavior.")]
    [SerializeField] private Transform[] m_PatrolPoints;
    [Tooltip("Patrol movement speed in meters per second.")]
    [SerializeField] private float m_PatrolSpeed = 2.0f;
    [Tooltip("Amount of time to pause between movement once reached a patrol point.")]
    [SerializeField] private float m_WaitDuration = 1.5f;


    // ===== PUBLIC FIELDS ===== //

    /// <summary>
    /// Exposed variable to retrieve the current movement direction.
    /// </summary>
    public Vector2 MoveDirection => ((Vector3)m_Target - transform.position).normalized;


    // ===== PRIVATE FIELDS ===== //

    private Rigidbody2D m_Rigidbody;
    private SpriteAnimator m_Animator;
    private Vector2 m_HomePosition;
    private Vector2 m_Target;
    private int m_CurrentPatrolIdx = 0;
    private bool m_IsPaused = false;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<SpriteAnimator>();
        m_HomePosition = transform.position;
        StartCoroutine(SetNewPatrolPoint());
    }

    private void OnEnable()
    {
        StartCoroutine(SetNewPatrolPoint());
    }

    void FixedUpdate()
    {
        if (m_IsPaused)
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = MoveDirection * m_PatrolSpeed;
        m_Rigidbody.AddForce(direction - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);

        if (Vector2.Distance(transform.position, m_Target) < 0.1f)
        {
            StartCoroutine(SetNewPatrolPoint());
        }

        m_Animator.SetCurrentDirection(direction);
    }

    /// <summary>
    /// Waits an amount of time once the target is reached to choose a new target.
    /// </summary>
    /// <returns></returns>
    IEnumerator SetNewPatrolPoint()
    {
        m_IsPaused = true;
        m_Animator.StartAnimation("Idle");
        yield return new WaitForSeconds(m_WaitDuration);

        m_CurrentPatrolIdx = Random.Range(0, m_PatrolPoints.Length);
        m_Target = m_PatrolPoints[m_CurrentPatrolIdx].localPosition;
        m_Animator.StartAnimation("Walk");
        m_IsPaused = false;
    }
}
