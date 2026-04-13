using UnityEngine;

public class AngryEchoAi : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    private AudioSource EchoDeathScream;

    public GameObject Hitbox;
    public bool FacePlayer;
    public Transform self;
    public Collision2D InRangeCollider;

    [HideInInspector]
    public bool Bcollision;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        EchoDeathScream = GetComponent<AudioSource>();
        self = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (FacePlayer) 
        { 
            Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
            // Calculate the direction from the trigger (this.transform.position) 
            // to the other object (other.transform.position)
            Vector2 directionToTarget = player.transform.position - this.transform.position;

            // Normalize the vector to get only the direction with a magnitude (length) of 1
            Vector2 normalizedDirection = directionToTarget.normalized;

            // You can now use normalizedDirection for various purposes
            //Debug.Log("Direction to " + collision.gameObject.name + ": " + normalizedDirection);

            if (normalizedDirection.x < -0.1f)
            {
                // Sets the rotation exactly to 0, 180, 0
                transform.eulerAngles = new Vector2(0, 180);
            }
            else if (normalizedDirection.x > 0.1f)
            {
                // Resets to face forward when moving right
                transform.eulerAngles = new Vector2(0, 0);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log(collision.gameObject);
            Bcollision = true;
            Debug.Log("Ai Collision");
            animator.SetBool("IsAttacking", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
       Bcollision = false;
    }

    private void TurnOnHitbox()
    {
        Hitbox.SetActive(true);
        Invoke(nameof(TurnOffHitbox), 1);
    }

    private void TurnOffHitbox()
    {
        Hitbox.SetActive(false);
        animator.SetBool("IsAttacking", false);
    }

    private void PlayDeath()
    {
        EchoDeathScream.Play();
    }
}
