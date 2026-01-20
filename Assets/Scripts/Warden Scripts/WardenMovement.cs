using Unity.VisualScripting;
using UnityEngine;

public class WardenMovement : MonoBehaviour
{
    public float speed;
    private bool isChasing;

    private Rigidbody2D rb;
    private Transform player;

    public bool stunned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }

        if (stunned == true)
        {
            Invoke(nameof(Unstun), 2);
        }
    }

    private void Unstun()
    {
        stunned = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player == null && !stunned)
            {
                player = collision.transform;
            }
            isChasing = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.velocity = Vector2.zero;
            isChasing = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isChasing = false;
        rb.velocity = Vector2.zero;
    }
}
