using UnityEngine;
using System.Collections.Generic;

public class EnemyMelee : MonoBehaviour
{
    public int damage = 5;
    public float attackCooldown = 1.5f;

    private float cooldownTimer = 0f;
    private HashSet<GameObject> playersInRange = new HashSet<GameObject>();

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // Attempt attack if player is nearby and cooldown is ready
        if (cooldownTimer <= 0f && playersInRange.Count > 0)
        {
            foreach (var player in playersInRange)
            {
                var health = player.GetComponent<NPA_Health_Components.Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    cooldownTimer = attackCooldown;
                    break; // Only hit one player per cooldown
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.gameObject);
        }
    }
}
