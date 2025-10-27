using NPA_PlayerPrefab.Scripts;
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
    /// AUTHOR: Jose Escobedo
    /// </summary>
    private Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets the appropriate base animation depending on the incoming velocity.
    /// </summary>
    /// <param name="velocity">Caller object's velocity.</param>
    public void SetAnimBasedOnSpeed(Vector3 velocity)
    {
        m_Animator.SetFloat("velocity", velocity.magnitude);
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
        m_Animator.SetTrigger("isAttacking");
    }

    /// <summary>
    /// Called once when the player was hit by anything.
    /// </summary>
    public void OnPlayerHit()
    {
        m_Animator.SetTrigger("isHit");
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
