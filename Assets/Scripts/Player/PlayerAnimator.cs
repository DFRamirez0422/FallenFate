using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    // ===== PUBLIC FIELDS ===== //

    /// <summary>Last movement direction quantized to 8 directions, for combat and facing.</summary>
    public Vector2 LastMovedDirection => m_LastMovedDirection;


    // ===== PRIVATE FIELDS ===== //

    /// <summary>
    /// Enumeration to express the current animation state.
    /// </summary>
    enum State
    {
        Idle,
        Walk,
        Attack,
        Damage,
        Death
    }

    /// <summary>
    /// Animation controller manager for the player. Other components call in when something
    /// requires animation to change. AUTHOR: Jose Escobedo
    /// </summary>
    private Animator m_Animator;
    private Vector2 m_LastMovedDirection = Vector2.down;
    private State m_CurrentState = State.Idle;

    // Boolean flag to track whether a new animation state is reached.
    private bool m_IsNeedUpdate = true;

    // Angle limit for the facing direction during horizontal movements.
    private const float m_HorizontalAngleLimit = 0.8f;

    // Angle limit for the facing direction during vertical movements.
    private const float m_VerticalAngleLimit = 0.3f;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!m_IsNeedUpdate) return;

        // m_Animator = GetComponent<Animator>(); /// NOTE: Already set in Awake, no need to get every frame
        Vector2 dir = m_LastMovedDirection.normalized;

        if (dir.x < -m_HorizontalAngleLimit) // LEFT
        {
            switch(m_CurrentState)
            {
                case State.Idle: default: m_Animator.Play("Idle_Left"); break;
                case State.Walk: m_Animator.Play("Walk_Left"); break;
                case State.Attack: m_Animator.Play("Attack_Left"); break;
                case State.Damage: m_Animator.Play("Hurt_Left"); break;
                case State.Death: m_Animator.Play("Death_Left"); break;
            }
        }
        else if (dir.x > m_HorizontalAngleLimit) // RIGHT
        {
            switch(m_CurrentState)
            {
                case State.Idle: default: m_Animator.Play("Idle_Right"); break;
                case State.Walk: m_Animator.Play("Walk_Right"); break;
                case State.Attack: m_Animator.Play("Attack_Right"); break;
                case State.Damage: m_Animator.Play("Hurt_Right"); break;
                case State.Death: m_Animator.Play("Death_Right"); break;
            }
        }
        else if (dir.y > m_VerticalAngleLimit) // UP
        {
            switch(m_CurrentState)
            {
                case State.Idle: default: m_Animator.Play("Idle_Back"); break;
                case State.Walk: m_Animator.Play("Walk_Back"); break;
                case State.Attack: m_Animator.Play("Attack_Back"); break;
                case State.Damage: m_Animator.Play("Hurt_Back"); break;
                case State.Death: m_Animator.Play("Death_Back"); break;
            }
        }
        else if (dir.y < -m_VerticalAngleLimit) // DOWN
        {
            switch(m_CurrentState)
            {
                case State.Idle: default: m_Animator.Play("Idle_Forward"); break;
                case State.Walk: m_Animator.Play("Walk_Forward"); break;
                case State.Attack: m_Animator.Play("Attack_Forward"); break;
                case State.Damage: m_Animator.Play("Hurt_Forward"); break;
                case State.Death: m_Animator.Play("Death_Forward"); break;
            }
        }

        m_IsNeedUpdate = false;
    }

    /// <summary>Update animator speed for walking/idle.</summary>
    public void SetCurrentSpeed(float speed)
    {
        m_Animator.SetFloat("MoveSpeed", speed);
        const float threshold = 0.1f;

        if (m_CurrentState == State.Walk && speed < threshold)
        {
            m_CurrentState = State.Idle;
            m_IsNeedUpdate = true;
        }
        else if (m_CurrentState == State.Idle && speed > threshold)
        {
            m_CurrentState = State.Walk;
            m_IsNeedUpdate = true;
        }
    }

    /// <summary>Update animator direction and store last direction for combat.</summary>
    public void SetCurrentDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            m_LastMovedDirection = QuantizeTo8(direction);

        // Check if the input direction has changed dramatically from the stored direction.
        const float dot_threshold = 0.1f;
        if (Mathf.Abs(Vector3.Dot(m_LastMovedDirection, direction)) > dot_threshold)
        {
            m_IsNeedUpdate = true;
        }

        m_Animator.SetFloat("LastDirX", direction.normalized.x);
        m_Animator.SetFloat("LastDirY", direction.normalized.y);
    }

    private static Vector2 QuantizeTo8(Vector2 dir)
    {
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    /// <summary>
    /// Starts the animation state indicating the player has started an attack.
    /// </summary>
    public void StartAttack()
    {
        m_CurrentState = State.Attack;
        m_IsNeedUpdate = true;
    }

    /// <summary>
    /// Starts the animation state indicating the player has been hit and damaged.
    /// </summary>
    public void StartDamage()
    {
        m_CurrentState = State.Damage;
        m_IsNeedUpdate = true;
    }

    /// <summary>
    /// Starts the animation state indicating the player has entered death.
    /// </summary>
    public void StartDeath()
    {
        m_CurrentState = State.Death;
        m_IsNeedUpdate = true;
    }

    /// <summary>Play an animation by state name.</summary>
    public void StartAnimation(string name)
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play(name);
    }

    /// <summary>Reset to movement blend tree.</summary>
    public void Reset()
    {
        m_CurrentState = State.Idle;
        m_IsNeedUpdate = true;
    }
}
