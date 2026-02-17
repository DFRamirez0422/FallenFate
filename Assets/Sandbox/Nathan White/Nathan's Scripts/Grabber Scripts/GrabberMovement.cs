using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GrabberMovement : MonoBehaviour
{
    public float speed;
    public bool isChasing;
    [SerializeField]
    public static bool SomeoneGrabbedPlayer = false;

    public Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    private ButtonMash ButtonMash;
    private Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        animator.SetBool("Chasing", false);
        ButtonMash = GetComponentInParent<ButtonMash>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == true && !animator.GetBool("Attacking") && SomeoneGrabbedPlayer == false)
        {
            direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        if (ButtonMash.started == true)
        {
            SomeoneGrabbedPlayer = true;
        }

        if (SomeoneGrabbedPlayer == true)
        {
            direction = transform.position;
            rb.linearVelocity = Vector2.zero;
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


            // Calculate the direction from the trigger (this.transform.position) 
            // to the other object (other.transform.position)
            Vector2 directionToTarget = collision.transform.position - this.transform.position;

            // Normalize the vector to get only the direction with a magnitude (length) of 1
            Vector2 normalizedDirection = directionToTarget.normalized;

            // You can now use normalizedDirection for various purposes
            Debug.Log("Direction to " + collision.gameObject.name + ": " + normalizedDirection);

            animator.SetFloat("DirX", normalizedDirection.x);
            animator.SetFloat("DirY", normalizedDirection.y);
        }

        if (collision.gameObject.tag == "Player" && SomeoneGrabbedPlayer == true)
        {
            isChasing = false;
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

    public void StoppedGrabbing()
    {
        SomeoneGrabbedPlayer = false;
    }
}
