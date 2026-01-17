using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    /// <summary>
    /// Animatation controller manager class for the player.
    /// 
    /// This class bridges the connection between the player and player animations. The imagined use
    /// of this class is to have other player components/scripts call in when something interesting happens
    /// that requires animation to be changed. Callers are required to return data pertinent for
    /// the animation to be judged if any arguments are present.
    /// 
    /// AUTHOR: Jose Escobedo
    /// </summary>
    private Animator m_Animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Update the animator sprite based on the current movement speed for walking and runnning.
    /// 
    /// Plays the player walking animtion while the player MoveSpeed is Greator than 0. And plays idle animation when player MoveSpeed is 0 or less.
    /// </summary>
    /// <param name="speed">Speed of the entity in meters per second.</param>
    public void SetCurrentSpeed(float speed)
    {
        m_Animator.SetFloat("MoveSpeed", speed);
    }

    /// <summary>
    /// Update the animator sprite direction based on the input vector.
    /// </summary>
    /// <param name="direction">Input direction vector. Do not normalize because the input magnitude is used.</param>
    public void SetCurrentDirection(Vector2 direction)
    {
        // Sprite flipping handling. Check the input direction but only if the player is actively moving.
        if (direction.x < 0.1f)
        {
            gameObject.transform.localScale = new Vector2(-1.0f, 1.0f);
        }
        else if (direction.x > 0.1f)
        {
            gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
        }
    }

    /// <summary>
    /// Directly plays a given animation clip by its defined name.
    /// </summary>
    /// <param name="name"></param>
    public void StartAnimation(string name)
    {
        m_Animator.Play(name);
    }
}
