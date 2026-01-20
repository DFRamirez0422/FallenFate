using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Tooltip("Amount of hit pints for the entity to start with as well as its maximum health limit.")]
    [SerializeField] private int m_StartingHealth;
    [Tooltip("Event to be invoked when healing, i.e. increasing hit points.")]
    [SerializeField] private UnityEvent m_OnHeal;
    [Tooltip("Event to be invoked when hit, i.e. decreasing hit points.")]
    [SerializeField] private UnityEvent m_OnHit;
    [Tooltip("Event to be invoked upon reaching zero hit points.")]
    [SerializeField] private UnityEvent m_OnZeroHealth;
    private int m_CurrentHealth;

    /// <summary>
    /// Exposed public variable for the current number of hit points.
    /// </summary>
    public int CurrentHealth => m_CurrentHealth;

    /// <summary>
    /// Exposed public variaable for the maximum number of hit points.
    /// </summary>
    public int MaxHealth => m_StartingHealth;

    void Start()
    {
        m_CurrentHealth = m_StartingHealth;
    }

    /// <summary>
    /// Main function to be called by outside code to change the entity's health by an amount.
    /// </summary>
    /// <param name="amount">Amount of hit points to channge the current health by.
    /// Negative numbers are damange, positive numbers are healing.</param>
    public void ChangeHealth(int amount)
    {
        if (amount > 0)
        {
            Heal(amount);
        }
        else if (amount < 0)
        {
            Hit(-amount);
        }
    }

    /// <summary>
    /// Heal the entity's health by a certain amount.
    /// </summary>
    /// <param name="amount">Amount of hit points to change the current health by.</param>
    public void Heal(int amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth > MaxHealth)
        {
            m_CurrentHealth = MaxHealth;
        }

        m_OnHeal?.Invoke();
    }

    /// <summary>
    /// Hurt the entity's health by a certain amount.
    /// </summary>
    /// <param name="amount">Amount of hit points to change the current health by.</param>
    public void Hit(int amount)
    {
        m_CurrentHealth -= amount;
        
        if (m_CurrentHealth <= 0)
        {
            m_OnZeroHealth?.Invoke();
        }
        else
        {
            m_OnHit?.Invoke();
        }
    }
}
