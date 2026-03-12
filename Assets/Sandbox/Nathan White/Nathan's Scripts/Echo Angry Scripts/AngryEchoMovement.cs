using Unity.VisualScripting;
using UnityEngine;

public class AngryEchoMovement : MonoBehaviour
{
    public AngryEchoAi brain;
    private Transform player;
    public float speed = 0.001f;
    public Rigidbody2D rb;
    public bool CanMove, Triggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        brain = rb.GetComponentInChildren<AngryEchoAi>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove && Triggered && !brain.animator.GetBool("IsAttacking"))
        {
            Debug.Log("Should Move");
            brain.self.position = Vector2.MoveTowards(brain.self.position, player.position, speed);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.transform;
            Triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Triggered = false;
    }
}
