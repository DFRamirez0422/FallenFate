using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    [Header("Heart Images")]
    public Image[] hearts;

    private PlayerHealth m_Health;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_Health = player.GetComponent<PlayerHealth>();

        UpdateHearts();
    }

    void Update()
    {
        currentHealth = m_Health.CurrentHealth;
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}
