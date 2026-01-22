using Unity.Mathematics;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Amount of hit pints for the maximum health limit.")]
    [SerializeField] private int m_MaxHealth;
    private int m_CurrentHealth;

    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    public void ChangeHealth(int amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth > m_MaxHealth)
        {
            m_CurrentHealth = m_MaxHealth;
        }
        else if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
