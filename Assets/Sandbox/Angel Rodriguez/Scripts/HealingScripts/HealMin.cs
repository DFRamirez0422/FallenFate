using UnityEngine;
using NUnit.Framework;

public class HealMin : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerMin;
    bool Colliding;

    private void Start()
    {
        Colliding = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Colliding) { return; }
            Colliding = true;
            HealPowerMin = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (HealPowerMin != null)
            {
                HealPowerMin.Heal(0.10f);
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
