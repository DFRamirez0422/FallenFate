using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public Slider slider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentHealth <= 0)
        {
            PowerUp_Droprates powerUpDropper = this.GetComponent<PowerUp_Droprates>();
            powerUpDropper.DropPowerUp();
            // Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        
    }

    public void TakeDamage(int damage)
    {
        // Subtract incoming damage from current health
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took damage {damage} damage. HP: {currentHealth}/{maxHealth}");
        slider.value = currentHealth;
    }
}
