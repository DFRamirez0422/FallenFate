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
            NPA_PlayerPrefab.Scripts.PlayerController playerController = other.gameObject.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();

            if (HealPowerMin != null)
            {
                if (playerController != null)
                {
                    playerController.IsHealing = true;
                    HealPowerMin.Heal(0.10f);
                    var copy = this.gameObject;
                    Destroy(copy);
                }
                else{}
            }
            else { }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

    }
}
