using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    /// <summary>
    /// Animation controller manager for the player. Other components call in when something
    /// requires animation to change. AUTHOR: Jose Escobedo
    /// </summary>
    private Animator m_Animator;
    private Vector2 m_LastMovedDirection = Vector2.down;

    /// <summary>Last movement direction quantized to 8 directions, for combat and facing.</summary>
    public Vector2 LastMovedDirection => m_LastMovedDirection;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    /// <summary>Update animator speed for walking/idle.</summary>
    public void SetCurrentSpeed(float speed)
    {
        m_Animator.SetFloat("MoveSpeed", speed);
    }

    /// <summary>Update animator direction and store last direction for combat.</summary>
    public void SetCurrentDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            m_LastMovedDirection = QuantizeTo8(direction);

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

    /// <summary>Play an animation by state name.</summary>
    public void StartAnimation(string name)
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play(name);
    }

    /// <summary>Reset to movement blend tree.</summary>
    public void Reset()
    {
        m_Animator.Play("Movement");
    }
}
