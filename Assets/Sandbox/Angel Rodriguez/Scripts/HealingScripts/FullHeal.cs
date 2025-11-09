using UnityEngine;

public class FullHeal : MonoBehaviour
{
    private NPA_Health_Components.Health HealPowerFull;
    bool Colliding;

    private void Start()
    {
        Colliding = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(Colliding) { return; }
            Colliding = true;
            HealPowerFull = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (HealPowerFull != null)
            {
                HealPowerFull.FullHeal();
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
