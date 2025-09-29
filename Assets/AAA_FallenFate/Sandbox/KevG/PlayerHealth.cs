// Player Health System
// Code by Luis Espinoza

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 8; // total health (each heart = 2 health)
    private int currentHealth; // keeps track of current health

    [Header("Heart Sprites")]
    public Sprite fullHeart;  // sprite for full heart
    public Sprite halfHeart;  // sprite for half heart
    public Sprite emptyHeart; // sprite for empty heart

    [Header("Heart Images")]
    public Image[] hearts; // drag your heart UI Images into this array

    void Start()
    {
        currentHealth = maxHealth; // start with full health
        UpdateHearts();            // update UI at start
        Debug.Log("Current Health: " + currentHealth); 
    }

    void Update()
    {
        // Test input for taking damage and healing
        
        if (Input.GetKeyDown(KeyCode.H)) // press H to take 1 damage (half heart)
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.J)) // press J to heal 1 health (half heart)
        {
            Heal(1);
        }
    }

    public void TakeDamage(int damage) // call this function to lose health
    {
        currentHealth -= damage; // subtract damage from health
        if (currentHealth < 0) currentHealth = 0; // don't go below 0

        UpdateHearts(); // refresh hearts UI
        Debug.Log("Current Health: " + currentHealth); 
    }

    public void Heal(int amount) // call this function to gain health
    {
        currentHealth += amount; // add healing to health
        if (currentHealth > maxHealth) currentHealth = maxHealth; // don't go above max

        UpdateHearts(); // refresh hearts UI
        Debug.Log("Current Health: " + currentHealth); 
    }

    private void UpdateHearts() // updates heart icons depending on current health
    {
        for (int i = 0; i < hearts.Length; i++) // loop through each heart slot
        {
            int heartHealth = currentHealth - (i * 2); // calculate remaining health for this slot

            if (heartHealth >= 2)
            {
                hearts[i].sprite = fullHeart; // full heart if 2 health left
            }
            else if (heartHealth == 1)
            {
                hearts[i].sprite = halfHeart; // half heart if 1 health left
            }
            else
            {
                hearts[i].sprite = emptyHeart; // empty if 0 health left
            }
        }
    }
}
