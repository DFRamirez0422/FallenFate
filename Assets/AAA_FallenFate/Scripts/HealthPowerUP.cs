using UnityEngine;
using NUnit.Framework;

public class HealthPowerUP : MonoBehaviour
{
    private NPA_Health_Components.Health OrbPower;
    private ElenaAI ElenaPickUp;
    private void Update()
    {
        var copy = this.gameObject;
        Destroy(copy, 10);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OrbPower = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (OrbPower != null)
            {
                OrbPower.GetHealth(15);
                Debug.Log("Player got Health");
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaPickUp = other.gameObject.GetComponent<ElenaAI>();
            if(ElenaPickUp != null)
            {
                ElenaPickUp.HealthPackHold = 1;
                var copyH = this.gameObject;
                Destroy(copyH);
            }
        }
    }
}
