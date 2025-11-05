using System.ComponentModel;
using NPA_PlayerPrefab.Scripts;
using UnityEditor.EditorTools;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    /// <summary>
    /// Animatation controller manager class for the player.
    /// 
    /// This class bridges the connection between the player and player animations. The imagined use
    /// of this class is to be called by other player scripts whenever something interesting happens
    /// that requires animation to be changed. Callers are required to pass in data pertinent for
    /// the animation to be judged if any arguments are present.
    /// 
    /// Ideally, I would like for this class to refer to outside classes in a sort of:
    /// Animator (Observer) --> PlayerCombat/PlayerRhythm (Subject)
    /// ...manner for better modularity. However, neither of these two classes have any usable public
    /// fields that make this relationship workable. Even a classic decorator pattern wouldn't work
    /// because of the private access. For now, outside code has to call into the Animator and manually
    /// call methods. Bit intrusive, wouldn't you say?
    /// 
    /// I suppose if I have full control over my own sandbox, I can just make certain details public.
    /// 
    /// UPDATE: 2025-11-03 13:47 : Made my sandbox copies have public fields. Should be better.
    /// 
    /// AUTHOR: Jose Escobedo
    /// </summary>

    [Header("Player Components")]
    [Tooltip("Player controller component.")]
    [SerializeField] private PlayerControllerAnimatorAndDebug m_PlayerController;
    [Tooltip("Player combat component.")]
    [SerializeField] private PlayerCombatRhythmAnimatorAndDebug m_PlayerCombat;

    private Animator m_Animator;

    void Start()
    {
        if (!m_Animator) m_Animator = GetComponent<Animator>();
        
        if (!m_PlayerController)
        {
            TryGetComponent<PlayerControllerAnimatorAndDebug>(out m_PlayerController);
        }
        if (!m_PlayerCombat)
        {
            TryGetComponent<PlayerCombatRhythmAnimatorAndDebug>(out m_PlayerCombat);
        }
    }

    void Update()
    {
        if (!m_PlayerController) return;
        if (!m_PlayerCombat) return;

        SetAnimBasedOnSpeed(m_PlayerController.Velocity);

        if (m_PlayerController.IsDashing)
        {
            SetPlayerIsDashing();
        }

        if (m_PlayerCombat.IsAttacking)
        {
            SetPlayerIsAttacking(m_PlayerCombat.CurrentAttack);
        }

        // Currently missing the following items to incorporate. However, they're not in the codebase I have.
        // TODO: if the missing features are present on the player prefab used for the final game, please let me know!
        //
        // missing : isHit
        // missing : isDead
        // missing : isPerryBlock
    }

    /// <summary>
    /// Sets the appropriate base animation depending on the incoming velocity.
    /// </summary>
    /// <param name="velocity">Caller object's velocity in ms/s.</param>
    public void SetAnimBasedOnSpeed(float velocity)
    {
        m_Animator.SetFloat("velocity", velocity);
    }

    /// <summary>
    /// Called each frame when the player is currently in a dash attack.
    /// </summary>
    public void SetPlayerIsDashing()
    {
        m_Animator.SetTrigger("isDashing");
    }

    /// <summary>
    /// Called each frame when the player is in the middle of an attack or finisher.
    /// </summary>
    /// <param name="attackData">Attack data that decides what animation to enable based on internal values.</param>
    public void SetPlayerIsAttacking(AttackData attackData)
    {
        // Apparently there is some sort of bug of this triggering longer than it has to.
        // Commenting this line out makes everything work despite the trigger being a requirement.
        //m_Animator.SetTrigger("isAttacking");

        // Side note: is the naming convention any good? It's a set of triggers for all possible attacks,
        // but the naming looks a bit funny. It is a low-level detail anyway.
        m_Animator.SetTrigger($"used_{attackData.attackName}");
    }

    /// <summary>
    /// Called each frame when the player was hit and not able to move due to knockback.
    /// </summary>
    public void SetPlayerIsHit()
    {
        m_Animator.SetTrigger("isHit");
    }

    /// <summary>
    /// Called each frame when the player is currently in a perry block.
    /// </summary>
    public void SetPlayerParryBlock()
    {
        m_Animator.SetTrigger("isPerryBlock");
    }

    /// <summary>
    /// Called once when the player lost all hit points.
    /// </summary>
    public void OnPlayerDeath()
    {
        m_Animator.SetBool("isDead", true);
    }
    
    /// <summary>
    /// Called once when the player spawns into the scene.
    /// </summary>
    public void OnPlayerSpawn()
    {
        m_Animator.SetBool("isDead", false);
    }
}
