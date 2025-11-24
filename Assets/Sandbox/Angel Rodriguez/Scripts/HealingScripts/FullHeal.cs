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
        bool touchedGround = false;
        if (other.gameObject.CompareTag("Player"))
        {
            if(Colliding) { return; }
            Colliding = true;
            HealPowerFull = other.gameObject.GetComponent<NPA_Health_Components.Health>();
             NPA_PlayerPrefab.Scripts.PlayerController playerController = other.gameObject.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
            if (HealPowerFull != null)
            {
                if (playerController != null)
                {
                    playerController.IsHealing = true;
                    var copy = this.gameObject;
                    Destroy(copy);
                }
                else {}
            }
            else {}
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && !touchedGround)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            touchedGround = true;
        }
    }

}
