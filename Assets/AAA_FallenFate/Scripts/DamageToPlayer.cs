using NPA_Health_Components;
using UnityEngine;

public class DamageToPlayer : MonoBehaviour
{
    public int damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Health playerDamage = other.gameObject.GetComponent<Health>();
            if (playerDamage != null) { playerDamage.TakeDamage(damage); }
        }
    }
}
