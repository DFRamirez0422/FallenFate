using UnityEngine;

public class AngryEchoHitbox : MonoBehaviour
{
    private PlayerHealth health;

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
            health.TakeDamage(1,transform);
        }
    }
}
