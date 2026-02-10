using UnityEngine;

public class GrabberMovement : MonoBehaviour
{
    public float speed;
    private bool isChasing;

    [HideInInspector]
    public Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("Chasing", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            if (player == null)
            {
                player = collision.transform;
            }
            animator.SetBool("Chasing", true);
            isChasing = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("Chasing", false);
            isChasing = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Stop");
    }
}
