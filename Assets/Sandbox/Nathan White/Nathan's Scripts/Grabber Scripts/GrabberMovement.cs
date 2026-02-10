using UnityEngine;

public class GrabberMovement : MonoBehaviour
{
    public float speed;
    public bool isChasing;


    public Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        animator.SetBool("Chasing", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == true && !animator.GetBool("Attacking"))
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }


        if (animator.GetBool("Attacking"))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           
            if (player == null)
            {
                player = collision.transform;
                isChasing = false;
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
