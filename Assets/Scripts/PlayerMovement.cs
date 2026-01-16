using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// Main class for all player movement via the Input Handling.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteAnimator))]

public class PlayerMovement : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Tooltip("Normal walking speed in meters per second.")]
    [SerializeField] private float m_WalkSpeed = 5.0f;
    
    public PlayerCombat m_playerCombat;


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
    private SpriteAnimator m_Animator;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<SpriteAnimator>();
    }

    void FixedUpdate()
    {
        Vector2 input_axes = Vector2.ClampMagnitude(CurrentInput, 1f) * m_WalkSpeed; // Normalize diagonal movement
        m_Rigidbody.AddForce(input_axes - m_Rigidbody.linearVelocity, ForceMode2D.Impulse);
        
        m_Animator.SetCurrentSpeed(input_axes.magnitude);
        m_Animator.SetCurrentDirection(input_axes);
    }
}