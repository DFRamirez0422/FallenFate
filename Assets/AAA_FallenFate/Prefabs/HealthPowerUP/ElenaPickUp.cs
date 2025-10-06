using UnityEngine;

public class ElenaPickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI ElenaPickUpH = other.gameObject.GetComponent<ElenaAI>();

            if (ElenaPickUpH != null)
            {
                ElenaPickUpH.HealthPackHold = 1;
                var copyH = this.gameObject;
                Destroy(copyH);
            }

        }
    }
}
