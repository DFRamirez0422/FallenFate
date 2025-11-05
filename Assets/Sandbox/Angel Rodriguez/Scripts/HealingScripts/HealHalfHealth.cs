using UnityEngine;

public class HealHalfHealth : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerHalf;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealPowerHalf = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (HealPowerHalf != null)
            {
                HealPowerHalf.HalfHeal(0.50f);
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
