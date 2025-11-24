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
            NPA_PlayerPrefab.Scripts.PlayerController playerController = other.gameObject.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
            
            if (HealPowerHalf != null)
            {
                if (playerController != null)
                {
                    playerController.IsHealing = true;
                    var copy = this.gameObject;
                    Destroy(copy);
                }
                else{}
            }
            else{}
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }


}
