using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Amount of hit pints for the entity to start with as well as its maximum health limit.")]
    [SerializeField] private int m_MaxHealth;
    [Tooltip("Event to be invoked upon reaching zero hit points.")]
    [SerializeField] private UnityEvent m_OnZeroHealth;
    [Tooltip("Game over screen prefab to be displayed during a game over.")]
    [SerializeField] private GameObject m_GameOverScreenPrefab;
    private int m_CurrentHealth;

    /// <summary>
    /// Exposed public variable for the current number of hit points.
    /// </summary>
    public int CurrentHealth => m_CurrentHealth;

    /// <summary>
    /// Exposed public variaable for the maximum number of hit points.
    /// </summary>
    public int MaxHealth => m_MaxHealth;

    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    /// <summary>
    /// Main function to be called by outside code to change the entity's health by an amount.
    /// </summary>
    /// <param name="amount">Amount of hit points to channge the current health by.
    /// Negative numbers are damange, positive numbers are healing.</param>
    public void ChangeHealth(int amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth <= 0)
        {
            m_OnZeroHealth?.Invoke();
        }
    }

    /// <summary>
    /// Function to be called once the player reaches zero health or otherwise forced to die.
    /// </summary>
    public void StartPlayerDeath()
    {
        // Disable the player movement component.
        GetComponent<PlayerMovement>().Disable();

        // Start the death animation.
        GetComponent<PlayerAnimator>().StartAnimation("Death");

        // Very cheap hack to get around prefabs limitation of not invoking a callback of another prefab.
        // Yes, yell at me all you want about this horrendous coupling but it's not like I have another choice.
        // None of these prefabs know each other and they can't invoke one another's functions.
        GameObject game_over_screen = Instantiate(m_GameOverScreenPrefab);
        game_over_screen.GetComponent<GameOverScreen>().DisplayScreen();
    }

    /// <summary>
    /// Function to be called after a death sequence to reset the player's health back to its initial state.
    /// </summary>
    public void ResetHealth()
    {
        m_CurrentHealth = m_MaxHealth;
    }
}
