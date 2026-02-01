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
    /// Exposed variable to retrieve the current raw input axes values. Using CurrentDirection is preferrable to retrieve
    /// the current player movement direction.
    /// </summary>
    public Vector2 CurrentInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    /// <summary>
    /// Exposed variable to retrieve the player's current direction, including correcting for diagonal movements.
    /// </summary>
    // Normalize diagonal movement to prevent faster diagonal speed
    public Vector2 CurrentDirection => Vector2.ClampMagnitude(CurrentInput, 1f);

    // ===== PRIVATE FIELDS ===== //
    private Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_Animator;
    private PlayerCombat m_PlayerCombat;
    private Vector2 m_LastInput = Vector2.right; // Save the last movement direction once the player stops moving.
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
            Vector2 input_axes = CurrentDirection * m_WalkSpeed;
            m_Rigidbody.AddForce(input_axes - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);

            // Save the last movement direction only if the player is currently moving.
            // This helps keep the animation consistent as requested by David.
            if (CurrentDirection.magnitude > 0.1f)
            {
                m_LastInput = CurrentDirection;
            }

            m_Animator.SetCurrentSpeed(input_axes.magnitude);
            m_Animator.SetCurrentDirection(m_LastInput);
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

    /// <summary>
    /// Sets the player position to a respawn point. Useful for death, map transition, or other sequences.
    /// </summary>
    public void RespawnPlayer()
    {
        StartCoroutine(SetToRespawnPoint());
    }

    /// <summary>
    /// Resets the player to an initial state and respawns them, such as restarting from a game over.
    /// </summary>
    public void ResetPlayer()
    {
        m_Animator.Reset();
        GetComponent<PlayerHealth>().ResetHealth();
        RespawnPlayer();
    }

    /// <summary>
    /// Implementation for respawning the player to a respawn point.
    /// 
    /// Because of the way Unity seems to work, you cannot set the respawn point in the same execution
    /// frame as the new scene being loaded in. Therefore we have to delay respawning for the next
    /// frame, hence the coroutine.
    /// </summary>
    IEnumerator SetToRespawnPoint()
    {
        yield return null;
        GameObject respawn_point = GameObject.FindGameObjectWithTag("Respawn");
        m_Rigidbody.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = true;

        if (respawn_point)
        {
            transform.position = respawn_point.transform.position;
        }
        else
        {
            Debug.Log("No respawn point found - defaulting to centre of map.");
            transform.position = Vector2.zero;
        }
    }
}
