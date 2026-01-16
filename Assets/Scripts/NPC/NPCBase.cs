using System.Runtime.CompilerServices;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// Main state machine script for the non playable characters.
/// </summary>
public class NPCBase : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    public enum NPCState
    {
        Default, Idle, Patrol, Wander, Talk,
    };

    [Tooltip("Initial state for this NPC entity.")]
    [SerializeField] private NPCState m_CurrentState = NPCState.Patrol;


    // ===== PUBLIC FIELDS ===== //


    // ===== PRIVATE FIELDS ===== //

    private NPCPatrol m_PatrolState;
    private NPCWander m_WanderState;
    private NPCTalk m_TalkState;
    private NPCState m_DefaultState;
    
    void Start()
    {
        m_PatrolState = GetComponent<NPCPatrol>();
        m_WanderState = GetComponent<NPCWander>();
        m_TalkState = GetComponent<NPCTalk>();

        m_DefaultState = m_CurrentState;
        SwitchState(m_CurrentState);
    }

    public void SwitchState(NPCState new_state)
    {
        m_CurrentState = new_state;
        m_PatrolState.enabled = new_state == NPCState.Patrol;
        m_WanderState.enabled = new_state == NPCState.Wander;
        m_TalkState.enabled = new_state == NPCState.Talk;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SwitchState(m_DefaultState);
        }
    }
}
