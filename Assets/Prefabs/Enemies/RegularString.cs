using NPA_Health_Components;
using UnityEngine;

public class RegularString : MonoBehaviour
{
    public Health PlayerHealth;
    public BossStringController bossController;
    public float lifeTimer = 1;


    private void Start()
    {
        lifeTimer = bossController.ShadowUptime;
    }
    // Update is called once per frame
    void Update()
    {
        lifeTimer = lifeTimer - Time.deltaTime * 4;

        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is 

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);

        GameObject otherObject = collision.gameObject;

        // Get a specific component from the colliding object
        PlayerHealth = otherObject.GetComponent<Health>();

        if (otherObject.CompareTag("Player"))
        {
            Debug.Log("Got Player");
            PlayerHealth.TakeDamage(40);
        }

    }
}
