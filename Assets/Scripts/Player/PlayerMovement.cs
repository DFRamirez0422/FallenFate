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
    [Header("Movemnt")]
    [Tooltip("Normal walking speed in meters per second.")]
    [SerializeField] private float m_WalkSpeed = 5.0f;

    [Tooltip("The initial facing directioon of the player upon spawning.")]
    [SerializeField] private Direction m_SpawnDirection = Direction.Down;


    // ===== PUBLIC FIELDS ===== //

    /// <summary>
    /// Used as the initial facing direction during spawning.
    /// </summary>
    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    };

    /// <summary>
    /// Exposed variable for callers to request whether or not the player is active in gameplay or inactive due
    /// to a cutscene or other sequence.
    /// </summary>
    public bool IsActive => m_IsEnabled;

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
    public Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_Animator;
    private PlayerCombat m_PlayerCombat;
    private PlayerSound m_PlayerSound;
    private Vector2 m_LastInput = Vector2.right; // Save the last movement direction once the player stops moving.
    private bool m_IsKnockedBack = false;
    private bool m_IsEnabled = false;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<PlayerAnimator>();
        m_PlayerCombat = GetComponent<PlayerCombat>();
        m_PlayerSound = GetComponent<PlayerSound>();
        m_IsEnabled = true;
        SetFaceDirection(m_SpawnDirection);
    }

    void FixedUpdate()
    {
        if (!m_IsEnabled)
            return;

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

            m_Animator.SetCurrentSpeed(CurrentInput.magnitude);
            m_Animator.SetCurrentDirection(m_LastInput);
        }
    }

    void Update()
    {
        if (!m_IsEnabled)
            return;

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
        m_Animator.StartDamage();
        if (m_PlayerSound != null)
            m_PlayerSound.PlayDamage();
        StartCoroutine(KnockbackCounter(stun_time));
    }

    IEnumerator KnockbackCounter(float stun_time)
    {
        yield return new WaitForSeconds(stun_time);
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_IsKnockedBack = false;
        m_Animator.Reset();
    }

    /// <summary>
    /// Sets the player position to a respawn point. Useful for death, map transition, or other sequences.
    /// </summary>
    public void RespawnPlayer()
    {
        StartCoroutine(SetToRespawnPoint());
        SetFaceDirection(m_SpawnDirection);
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
    /// Disables player control, movement, and collision for use in death and other such sequences.
    /// </summary>
    public void Disable()
    {
        m_IsEnabled = false;
        LayerMask enemy_mask = 1 << LayerMask.NameToLayer("Enemy");

        // Disable rigid body to disable all movement.
        GetComponent<Rigidbody2D>().Sleep();

        // Disable collider to let enemies know the player is no longer around.
        GetComponent<Collider2D>().excludeLayers |= enemy_mask;

        // Reset player movement speed.
        m_Rigidbody.linearVelocity = Vector2.zero;

        // Reset player animation.
        m_Animator.SetCurrentSpeed(0.0f);
    }

    /// <summary>
    /// Disables player control, movement, and collision for use in normal gameplay.
    /// </summary>
    public void Enable()
    {
        m_IsEnabled = true;
        LayerMask enemy_mask = 1 << LayerMask.NameToLayer("Enemy");

        GetComponent<Rigidbody2D>().WakeUp();
        GetComponent<Collider2D>().excludeLayers &= ~enemy_mask;
        m_Rigidbody.linearVelocity = Vector2.zero;
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

        // Re-enable the player componnents that may have been deactivated during respawn.
        Enable();

        // Find a suitable respawn point.
        // By default, we just locate the first thing Unity gives us.
        GameObject respawn_point = GameObject.FindGameObjectWithTag("Respawn");

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

    /// <summary>
    /// Sets the current facing direction basd on the  enumeration.
    /// </summary>
    /// <param name="dir"></param>
    private void SetFaceDirection(Direction dir)
    {
        // Set up some variables in relation to the starting face direction.
        switch (dir)
        {
            case Direction.Up: m_LastInput = Vector2.up; break;
            case Direction.Down: m_LastInput = Vector2.down; break;
            case Direction.Left: m_LastInput = Vector2.left; break;
            case Direction.Right: m_LastInput = Vector2.right; break;
        }
    }
}
