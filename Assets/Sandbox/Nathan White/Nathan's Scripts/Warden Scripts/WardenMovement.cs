using Unity.VisualScripting;
using UnityEngine;

public class WardenMovement : MonoBehaviour
{
    public float speed;
    private bool isChasing;
    private float scalingRadius;

    private Rigidbody2D WardensRigidBody;
    private Transform player;
    public CircleCollider2D triggercollider;
    [SerializeField]
    private float scalingRadiusSpeed = 0.001f; 

    public bool stunned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WardensRigidBody = GetComponent<Rigidbody2D>();
        triggercollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == false)
        {
            triggercollider.radius = scalingRadius;
            scalingRadius = scalingRadius + scalingRadiusSpeed;
        }
        if (isChasing == true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            WardensRigidBody.velocity = direction * speed;
        }

        if (stunned == true)
        {
            WardensRigidBody.velocity = Vector2.zero;
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
            WardensRigidBody.velocity = Vector2.zero;
            isChasing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isChasing = false;
        WardensRigidBody.velocity = Vector2.zero;
    }
}
