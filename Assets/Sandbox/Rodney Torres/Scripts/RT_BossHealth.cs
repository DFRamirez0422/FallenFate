using UnityEngine;

public class RT_BossHealth : MonoBehaviour
{
    [Tooltip("Maximum HP")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool invulnerable = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool Invulnerable => invulnerable;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable)
        {
            Debug.Log($"{gameObject.name} is invulnerable. No damage taken.");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount}. HP now {currentHealth}/{maxHealth}");
    }

    public void SetInvulnerable(bool state)
    {
        invulnerable = state;
        Debug.Log($"{gameObject.name} invulnerability set to {state}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}
