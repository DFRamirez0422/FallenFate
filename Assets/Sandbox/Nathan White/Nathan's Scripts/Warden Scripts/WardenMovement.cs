using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WardenMovement : MonoBehaviour
{
    public float speed;
    public bool isChasing;
    private float scalingRadius = 1;

    private Rigidbody2D WardensRigidBody;
    private Transform player;
    public CircleCollider2D triggercollider;
    private Animator animator;
    [SerializeField]
    private float scalingRadiusSpeed = 0.001f;

    public bool stunned, knocked;

    public GameObject Hitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        WardensRigidBody = GetComponent<Rigidbody2D>();
        triggercollider = GetComponentInChildren<CircleCollider2D>(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == false)
        {
            triggercollider.radius = scalingRadius;
            scalingRadius = scalingRadius + scalingRadiusSpeed;
        }
        if (isChasing == true && !knocked && !animator.GetBool("Attacking"))
        {
            Vector2 direction = (player.position - transform.position).normalized;
            WardensRigidBody.linearVelocity = direction * speed;
        }

        if (stunned == true)
        {
            WardensRigidBody.linearVelocity = Vector2.zero;
            Invoke(nameof(Unstun), 2);
        }
    }

    private void Unstun()
    {
        stunned = false;

        if (knocked == true)
        {
            knocked = false;
        }
        WardensRigidBody.linearVelocity = Vector2.zero;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Calculate the direction from the trigger (this.transform.position) 
            // to the other object (other.transform.position)
            Vector2 directionToTarget = collision.transform.position - this.transform.position;

            // Normalize the vector to get only the direction with a magnitude (length) of 1
            Vector2 normalizedDirection = directionToTarget.normalized;

            // You can now use normalizedDirection for various purposes
            //Debug.Log("Direction to " + collision.gameObject.name + ": " + normalizedDirection);

            animator.SetFloat("DirX", normalizedDirection.x);
            animator.SetFloat("DirY", normalizedDirection.y);


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
            WardensRigidBody.linearVelocity = Vector2.zero;
            isChasing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isChasing = false;
        WardensRigidBody.linearVelocity = Vector2.zero;

        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Attacking", true);
        }
    }

    public void NathansKnockbackClone()
    {
        knocked = true;

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Rigidbody2D grabberRB = GetComponent<Rigidbody2D>();

        Vector2 direction = (transform.position - playerTransform.position).normalized;

        // Apply knockback velocity
        grabberRB.AddForce(direction * 400, ForceMode2D.Impulse);

        Debug.Log("Knocked");

        Invoke(nameof(Unstun), 0.5f);
    }

    private void TurnOnHitbox()
    {
        Hitbox.SetActive(true);
        Invoke(nameof(TurnOffHitbox), 1);
    }

    private void TurnOffHitbox()
    {
        animator.SetBool("Attacking", false);
        Hitbox.SetActive(false);
    }
}
