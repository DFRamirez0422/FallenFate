using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main scrip for the talking behavior for the non playable characters.
/// </summary>
public class NPCTalk : MonoBehaviour
{
    
    // ===== USER INTERFACE FIELDS ===== //
    [SerializeField] private Animator m_InteractIconAnimator;
    [SerializeField] private List<DialogueSO> m_Conversations;
    [SerializeField] private DialogueSO m_CurrentConversation;


    // ===== PUBLIC FIELDS ===== //


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
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        m_Animator.StartAnimation("Idle");
        m_InteractIconAnimator.Play("Appear");
    }

    private void OnDisable()
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_InteractIconAnimator.Play("Disappear");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                CheckForNewConversation();
                DialogueManager.Instance.StartDialogue(m_CurrentConversation);
            }
        }
    }

    private void CheckForNewConversation()
    {
        for (int i = m_Conversations.Count - 1; i >= 0; i--)
        {
            var convo = m_Conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                m_Conversations.RemoveAt(i);
                m_CurrentConversation = convo;
            }
        }
    }
}
