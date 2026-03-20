using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Maximum hit points and starting health.")]
    [SerializeField] private int m_MaxHealth;
    [Tooltip("Event when healing.")]
    [SerializeField] private UnityEvent m_OnHeal;
    [Tooltip("Event when hit (damage).")]
    [SerializeField] private UnityEvent m_OnHit;
    [Tooltip("Event when reaching zero health.")]
    [SerializeField] private UnityEvent m_OnZeroHealth;
    [Tooltip("Game over screen prefab to display on death.")]
    [SerializeField] private GameObject m_GameOverScreenPrefab;

    public int m_CurrentHealth;
    public Transform LastHitSource {  get; private set; }
    public int CurrentHealth => m_CurrentHealth;
    public int MaxHealth => m_MaxHealth;
    
    private static bool initialized = false;

    private void Start()
    {
        if (!initialized)
        {
            m_CurrentHealth = m_MaxHealth;
            initialized = true;
        }
    }

    /// <summary>Change health by amount. Negative = damage, positive = heal.</summary>
    public void ChangeHealth(int amount)
    {
        if (amount > 0)
            Heal(amount);
        else if (amount < 0)
            TakeDamage(-amount, transform);
        
        if (m_CurrentHealth <= 0)
            m_OnZeroHealth?.Invoke();
    }

    /// <summary>
    /// Applies damage to the player and stores the source of the hit.
    /// This exists separately from ChangeHealth so damage events
    /// can track hit direction for knockback, reactions, etc.
    /// Damage must always be positive.
    /// </summary>
    public void TakeDamage(int damage, Transform hitSource)
    {
        if (damage <= 0) return;

        LastHitSource = hitSource;

        m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - damage);

        // Trigger hit reaction even if this damage is fatal
        m_OnHit?.Invoke();

        if (m_CurrentHealth == 0)
            m_OnZeroHealth?.Invoke();
    }

    private void Heal(int amount)
    {
        m_CurrentHealth += amount;
        if (m_CurrentHealth > MaxHealth)
            m_CurrentHealth = MaxHealth;
        m_OnHeal?.Invoke();
    }

    private void Hit(int amount)
    {
        m_CurrentHealth -= amount;
        if (m_CurrentHealth < 0)
            m_CurrentHealth = 0;
        if (amount > 0)
            m_OnHit?.Invoke();
    }

    /// <summary>Called when player reaches zero health (e.g. from m_OnZeroHealth).</summary>
    public void StartPlayerDeath()
    {
        // Disable the player movement component.
        GetComponent<PlayerMovement>().Disable();

        // Start the death animation.
        GetComponent<PlayerAnimator>().StartDeath();

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
