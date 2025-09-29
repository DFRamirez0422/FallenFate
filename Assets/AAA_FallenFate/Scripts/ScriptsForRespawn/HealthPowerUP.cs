using NPA_Health_Components;
using UnityEngine;

public class HealthPowerUP : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Health OrbPower = collision.gameObject.GetComponent<Health>();
            if (OrbPower != null)
            {
                OrbPower.GetHealth(10);
            }
            else { }
        }
    }
}
