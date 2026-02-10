using UnityEngine;

public class NPCIdle : MonoBehaviour
{
    // ===== PRIVATE FIELDS ===== //

    private Rigidbody2D m_Rigidbody;
    private PlayerAnimator m_Animator;
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<PlayerAnimator>();
    }

    private void OnEnable()
    {
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_Animator.StartAnimation("Idle");
    }
}
