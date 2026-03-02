using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Amount of hit points for the maximum health limit.")]
    [SerializeField] private int m_MaxHealth;
    [SerializeField] private UnityEvent m_OnHit;

    private int m_CurrentHealth;
    private EnemyHitScript m_HitReaction;
    private Animator animator;
    public float DeathDelay = 0;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
        m_HitReaction = GetComponent<EnemyHitScript>();
        animator = GetComponent<Animator>();
    }

    public void ChangeHealth(int amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth > m_MaxHealth)
            m_CurrentHealth = m_MaxHealth;
        else if (m_CurrentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetBool("Died", true);
                Destroy(gameObject, DeathDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        m_OnHit?.Invoke();
        
    }


}
