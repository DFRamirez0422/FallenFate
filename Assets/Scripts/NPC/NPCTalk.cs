using UnityEngine;

/// <summary>
/// Main scrip for the talking behavior for the non playable characters.
/// </summary>
public class NPCTalk : MonoBehaviour
{
    
    // ===== USER INTERFACE FIELDS ===== //


    // ===== PUBLIC FIELDS ===== //


    // ===== PRIVATE FIELDS ===== //

    private Rigidbody2D m_Rigidbody;
    private SpriteAnimator m_Animator;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<SpriteAnimator>();
    }

    private void OnEnable()
    {
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        m_Animator.StartAnimation("Idle");
    }

    private void OnDisable()
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
