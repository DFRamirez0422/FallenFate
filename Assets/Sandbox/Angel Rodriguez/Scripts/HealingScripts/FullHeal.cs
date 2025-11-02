using UnityEngine;

public class FullHeal : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerFull;
    private void Update()
    {
        var copy = this.gameObject;
        Destroy(copy, 10);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealPowerFull = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (HealPowerFull != null)
            {
                HealPowerFull.Heal(0.10f);
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
