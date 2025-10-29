using NPA_Health_Components;
using UnityEngine;

public class DamageToPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Health Damage = other.gameObject.GetComponent<Health>();
            if(Damage != null)
            {
                Damage.TakeDamage(100);
            }
        }
    }
}
