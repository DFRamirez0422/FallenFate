// Player Health System
using UnityEngine;
using UnityEngine.UI;
using NPA_Health_Components; // so we can access the Health script

public class HealthUI : MonoBehaviour
{

    [Header("References")]
    public Health playerHealth; // drag the Player (with Health script) into this slot

    [Header("Heart Settings")]
    public int heartsCount = 4; // number of hearts shown in UI (each heart = 2 health)

    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    [Header("Heart Images")]
    public Image[] hearts;

    private void Start()
    {
        UpdateHearts();
    }

    private void Update()
    {
        // Always sync UI with player health
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        // Convert player health (0–100) to heart health (0–heartsCount*2)
        int heartHealthValue = Mathf.RoundToInt(
            (float)playerHealth.CurrentHealth / playerHealth.MaxHealth * (heartsCount * 2)
        );

        for (int i = 0; i < hearts.Length; i++)
        {
            int heartHealth = heartHealthValue - (i * 2);

            if (heartHealth >= 2)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (heartHealth == 1)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}

