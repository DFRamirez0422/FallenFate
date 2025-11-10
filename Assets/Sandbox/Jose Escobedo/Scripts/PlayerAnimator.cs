using System.ComponentModel;
using AAA_FallenFate.Scripts.PlayerScripts;
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

    /// 
    /// How to properly integrate into the player system!
    /// 
    /// Firstly, replace the following class types that are on the sandbox into the ones used in production:
    /// PlayerController_JoseE -> PlayerController
    /// PlayerCombat_JoseE -> PlayerCombat
    /// Hitstun_JoseE -> Hitstun
    /// ParryBlock_JoseE -> ParryBlock
    /// Health_JoseE -> Health
    /// 
    /// Then, each of these modules must allow public fields to easily query certain wanted properties, such
    /// as current velocity, attack that was used, and whether or not the player was hit and/or dead.
    /// Specifically:
    ///     m_PlayerController.Velocity : current player velocity in m/s
    ///     m_PlayerHealth.IsDead : whether the player just died
    ///     m_PlayerHealth.IsTakenDamage : when the player cannot move due to damage hit
    ///     m_PlayerHitstun.IsStunned : when the player is currently hit stunned
    ///     m_PlayerController.IsDashing : when the player is currently in a dash 
    ///     m_PlayerCombat.IsAttacking : when the player is currently in an attack
    /// 
    [Header("Player Components")]
    [Tooltip("Player controller component.")]
    [SerializeField] private PlayerController_JoseE m_PlayerController;
    [Tooltip("Player combat component.")]
    [SerializeField] private PlayerCombat_JoseE m_PlayerCombat;
    [Tooltip("Player health component.")]
    [SerializeField] private NPA_Health_Components.Health_JoseE m_PlayerHealth;
    [Tooltip("Player hit stun component.")]
    [SerializeField] private Hitstun_JoseE m_PlayerHitstun;
    [Tooltip("Player parry block component.")]
    [SerializeField] private ParryBlock_JoseE m_PlayerParryBlock;

    private Animator m_Animator;

    void Start()
    {
        if (!m_Animator) m_Animator = GetComponent<Animator>();
        
        if (!m_PlayerController)
        {
            TryGetComponent(out m_PlayerController);
        }
        if (!m_PlayerCombat)
        {
            TryGetComponent(out m_PlayerCombat);
        }
        if (!m_PlayerHealth)
        {
            TryGetComponent(out m_PlayerHealth);
        }
    }

    void Update()
    {
        if (!m_PlayerController) return;

        SetAnimBasedOnSpeed(m_PlayerController.Velocity);

        if (m_PlayerHealth && m_PlayerHealth.IsDead)
        {
            OnPlayerDeath();
        }
        else if (m_PlayerHealth && m_PlayerHealth.IsTakenDamage)
        {
            ;
        }
        else if (m_PlayerHitstun && m_PlayerHitstun.IsStunned)
        {
            SetPlayerInHitStun();
        }
        else if (m_PlayerParryBlock && m_PlayerParryBlock.IsParryBlocking)
        {
            SetPlayerInParryBlock();
        }
        else if (m_PlayerController.IsDashing)
        {
            SetPlayerIsDashing();
        }
        else if (m_PlayerCombat && m_PlayerCombat.IsAttacking)
        {
            SetPlayerIsAttacking(m_PlayerCombat.CurrentAttack);
        }

        // Currently missing the following items to incorporate. However, they're not in the codebase I have.
        // TODO: if the missing features are present on the player prefab used for the final game, please let me know!
        //
        // missing:
        // OnPlayerSpawn
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
    public void SetPlayerInHitStun()
    {
        m_Animator.SetTrigger("inHitStun");
    }

    /// <summary>
    /// Called each frame when the player is currently in a perry block.
    /// </summary>
    public void SetPlayerInParryBlock()
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
