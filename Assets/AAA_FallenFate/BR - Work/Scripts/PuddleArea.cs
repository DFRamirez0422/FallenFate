using UnityEngine;
using UnityEngine;
using NPA_Health_Components;

public class PuddleArea : MonoBehaviour
{
    // Damage the player takes per second while standing in the puddle
    public float damagePerSecond = 5f;

    // How long the puddle stays active before disappearing
    public float duration = 5f;

    private void Start()
    {
        // Destroys the puddle after 'duration' seconds
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object staying in the puddle is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is inside the puddle.");

            // Try to get the player's Health component
            Health playerHealth = other.GetComponent<Health>();

            // If the player has a Health script, apply damage over time
            if (playerHealth != null)
            {
                int damage = Mathf.CeilToInt(damagePerSecond * Time.deltaTime);
                playerHealth.TakeDamage(damage);
            }
            else
            {
                // Warn in case the Player doesn’t have a Health component
                Debug.LogWarning("Health component not found on Player!");
            }
        }
    }
}
