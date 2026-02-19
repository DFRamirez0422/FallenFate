using UnityEngine;

public class AngryEchoHitbox : MonoBehaviour
{
    private PlayerHealth health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            health = collision.gameObject.GetComponent<PlayerHealth>();
            health.ChangeHealth(-1);
        }
    }
}
