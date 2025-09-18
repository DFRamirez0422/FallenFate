using UnityEngine;
using NPA_Health_Components;

public class CubeHurt : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    private Health playerHealth;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            print("player took projectile damage");
            Destroy(gameObject);
        }
    }
}
