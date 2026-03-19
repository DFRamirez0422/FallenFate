using UnityEngine;

public class WardenHitbox : MonoBehaviour
{
    private PlayerHealth health;
    public int damage = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.gameObject.tag == "Hitboxs")
        {
            Debug.Log("Should hit you");
            health.TakeDamage(damage, transform);
        }
    }
}
