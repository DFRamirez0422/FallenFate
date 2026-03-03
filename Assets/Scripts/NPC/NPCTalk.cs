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
    

    [Header("Audio Clips")]
    [Tooltip("Game object audio source for oneshot sound effects playback.")]
    [SerializeField] private AudioSource m_SoundPlayer;
    [Tooltip("")]
    [SerializeField] private AudioClip m_TalkSound;


    // ===== PUBLIC FIELDS ===== //


    // ===== PRIVATE FIELDS ===== //

    private Rigidbody2D m_Rigidbody;
    private Animator m_Animator;
    private bool m_AlreadyTalkedTo = false;

    private void Awake()
    {
        if (!m_SoundPlayer)
        {
            m_SoundPlayer = GetComponent<AudioSource>();
        }

        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        m_Animator.Play("Idle");
        m_InteractIconAnimator.Play("Appear");
        m_AlreadyTalkedTo = false;
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
            if (!m_AlreadyTalkedTo)
            {
                if (m_SoundPlayer && m_TalkSound)
                {
                    m_SoundPlayer.PlayOneShot(m_TalkSound);
                }

                CheckForNewConversation();
                
                if (m_CurrentConversation != null)
                {
                    DialogueManager.Instance.StartDialogue(m_CurrentConversation);
                    m_AlreadyTalkedTo = true;
                }
            }
            else if (!DialogueManager.Instance.IsDialogueActive)
            {
                m_AlreadyTalkedTo = false;
            }
        }
    }

    private void CheckForNewConversation()
    {
        for (int i = 0; i < m_Conversations.Count; i++)
        {
            var convo = m_Conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                // Consume only one conversation at a time to preserve intended order.
                m_Conversations.RemoveAt(i);
                m_CurrentConversation = convo;
                return;
            }
        }
    }
}
