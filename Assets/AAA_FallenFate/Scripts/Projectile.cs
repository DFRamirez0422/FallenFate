<<<<<<< HEAD
=======
using NPA_Health_Components;
>>>>>>> chore/attacksystem-organized-EA-MH-DG
using UnityEngine;

public class Projectile : MonoBehaviour
{
<<<<<<< HEAD
    public PlayerHealthN PlayerHealth;
    public float speed = 20f;
    public Rigidbody rb;
=======
    public Health PlayerHealth;
    public float speed = 20f;
    public Rigidbody rb;
    public int damage;
>>>>>>> chore/attacksystem-organized-EA-MH-DG

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
<<<<<<< HEAD
        PlayerHealth = otherObject.GetComponent<PlayerHealthN>();
=======
        PlayerHealth = otherObject.GetComponent<Health>();
>>>>>>> chore/attacksystem-organized-EA-MH-DG

        if (otherObject.CompareTag("Player"))
        {
            Debug.Log("Got Player");
<<<<<<< HEAD
            PlayerHealth.TakeDamage();
=======
            PlayerHealth.TakeDamage(damage);
>>>>>>> chore/attacksystem-organized-EA-MH-DG
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
