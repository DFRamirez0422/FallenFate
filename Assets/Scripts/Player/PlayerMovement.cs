using System.Collections;
using UnityEngine;

/// <summary>
/// Main class for all player movement via the Input Handling.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerMovement : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Normal walking speed in meters per second.")]
    [SerializeField] private float m_WalkSpeed = 5.0f;


    // ===== PUBLIC FIELDS ===== //
    /// <summary>
    /// Exposed variable to retrieve the player's current velocity in terms of meters per second.
    /// </summary>
    public float CurrentSpeed => m_Rigidbody.linearVelocity.magnitude;

    /// <summary>
    /// Exposed variable to retrieve the current raw input axes values.
    /// </summary>
    public Vector2 CurrentInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    // ===== PRIVATE FIELDS ===== //
    private Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_Animator;
    private PlayerCombat m_PlayerCombat;
    private bool m_IsKnockedBack;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<PlayerAnimator>();
        m_PlayerCombat = GetComponent<PlayerCombat>();
    }

    void FixedUpdate()
    {
        if (!m_IsKnockedBack)
        {
            // Normalize diagonal movement to prevent faster diagonal speed
            Vector2 input_axes = Vector2.ClampMagnitude(CurrentInput, 1f) * m_WalkSpeed;
            m_Rigidbody.AddForce(input_axes - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);

            m_Animator.SetCurrentSpeed(input_axes.magnitude);
            m_Animator.SetCurrentDirection(input_axes);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            m_PlayerCombat.Attack();
        }   
    }

    public void Knockback(Transform enemy, float force, float stun_time)
    {
        m_IsKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        m_Rigidbody.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stun_time));
    }

    IEnumerator KnockbackCounter(float stun_time)
    {
        yield return new WaitForSeconds(stun_time);
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_IsKnockedBack = false;
    }
}
