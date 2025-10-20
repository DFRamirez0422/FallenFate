using UnityEngine;
using NUnit.Framework;

public class HealthPowerUP : MonoBehaviour
{
    private NPA_Health_Components.Health OrbPower;
    private void Update()
    {
        var copy = this.gameObject;
        Destroy(copy, 10);      
    }

    private void FixedUpdate()
    {
        this.transform.Translate(GameObject.FindGameObjectWithTag("Player").transform.position * Time.deltaTime, Space.World);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OrbPower = other.gameObject.GetComponent<NPA_Health_Components.Health>();

            if (OrbPower != null)
            {
                OrbPower.Heal(15);
                Debug.Log("Player got Health");
                var copy = this.gameObject;
                Destroy(copy);
            }
            else { }
        }

    }
}
