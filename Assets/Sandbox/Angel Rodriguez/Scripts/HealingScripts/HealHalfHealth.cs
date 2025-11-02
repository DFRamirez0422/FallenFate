using UnityEngine;

public class HealHalfHealth : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerHalf;
    private void Update()
    {
        var copy = this.gameObject;
        Destroy(copy, 10);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealPowerHalf = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (HealPowerHalf != null)
            {
                HealPowerHalf.FullHeal();
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
