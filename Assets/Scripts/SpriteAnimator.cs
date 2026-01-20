using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpriteAnimator : MonoBehaviour
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

    /// <summary>
    /// Update the animator sprite based on the current movement speed for walking and runnning.
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
        if (direction.magnitude > 0.1f)
        {
            if (direction.x < 0.0f)
            {
                gameObject.transform.localScale = new Vector2(-1.0f, 1.0f);
            }
            else if (direction.x > 0.0f)
            {
                gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
            }
        }
    }
}
