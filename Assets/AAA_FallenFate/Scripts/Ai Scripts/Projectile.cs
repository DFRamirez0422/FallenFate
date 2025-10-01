using NPA_Health_Components;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Health PlayerHealth;
    public int damage = 20;
    public float speed = 20f;
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

        GameObject otherObject = collision.gameObject;
        

        // Get a specific component from the colliding object
        PlayerHealth = otherObject.GetComponent<Health>();

        if (otherObject.CompareTag("Player"))
        {
            Debug.Log("Got Player");
            PlayerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
