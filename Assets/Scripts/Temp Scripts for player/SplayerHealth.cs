using UnityEngine;

public class SplayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public GameObject DeathScreen;

    private PlayerMovement movement;


    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        DeathScreen.SetActive(false);
    }
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            movement.enabled = false;
            DeathScreen.SetActive(true);
        }
    }
}
