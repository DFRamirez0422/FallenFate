using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class DialogueTrigger : MonoBehaviour
{
    /// <summary>
    /// Script triggers that freeze the player and auto-play text without button interaction.
    /// 
    /// Trigger Sequence (When Player onTriggerEnter: an invisible box collider):
    /// 
    /// Freeze player movement.
    ///     Freeze all Enemy AI and Movement in the scene.
    ///     Open the dialogue textbox on the screen.
    ///     Go through the entire dialogue.
    /// 
    /// 
    /// Completion Sequence (Once it is finished—either through clicking OR skipping):
    ///     Un-Freeze player movement.
    ///     Un-Freeze all Enemy AI and Movement in the scene.
    /// </summary>

    // ===== USER INTERFACE FIELDS ===== //
    [SerializeField] private List<DialogueSO> m_Conversations;
    [Tooltip("Check whether or not the trigger should remain alive even after interaction.")]
    [SerializeField] private bool m_KeepTrigger = false;
    
    [Header("Audio Clips")]
    [Tooltip("")]
    [SerializeField] private AudioClip m_TalkSound;


    // ===== PUBLIC FIELDS ===== //


    // ===== PRIVATE FIELDS ===== //
    private AudioSource m_SoundPlayer;
    private DialogueSO m_CurrentConversation;
    private bool m_WasActiveDialogue = false;

    private void Awake()
    {
        m_SoundPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // If the end of the dialogue is reached and the keep trigger flag is disabled, destroy the object.
        if (m_WasActiveDialogue && !DialogueManager.Instance.IsDialogueActive)
        {
            if (!m_KeepTrigger)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_WasActiveDialogue = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !DialogueManager.Instance.IsDialogueActive)
        {    
            m_WasActiveDialogue = true;
            m_SoundPlayer.PlayOneShot(m_TalkSound);
            CheckForNewConversation();
            if (m_CurrentConversation != null)
            {
                DialogueManager.Instance.StartDialogue(m_CurrentConversation);
            }
        }
    }
}
