using Unity.VisualScripting;
using UnityEngine;

public class AngryEchoMovement : MonoBehaviour
{
    public AngryEchoAi brain;
    private Transform player;
    public float speed = 1f;
    public Rigidbody2D rb;
    public bool CanMove = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        brain = rb.GetComponentInChildren<AngryEchoAi>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("In trigger");
            player = collision.transform;
            if (CanMove)
            {
                Debug.Log("Should move");
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
        }
    }
}
