using UnityEngine;

public class HealHalfHealth : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerHalf;
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
            HealPowerHalf = other.gameObject.GetComponent<NPA_Health_Components.Health>();
            if (HealPowerHalf != null)
            {
                    HealPowerHalf.Heal(0.50f);
                    var copy = this.gameObject;
                    Destroy(copy);
            }
        }

    }
}
