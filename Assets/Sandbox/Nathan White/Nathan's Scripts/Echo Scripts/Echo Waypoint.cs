using UnityEngine;

public class EchoWaypoint : MonoBehaviour
{
    public Transform[] waypoints;

    [SerializeField]
    float moveSpeed = 2f;

    [HideInInspector]
    public int waypointIndex = 0;
    public bool IsMoving = false;

    private Animator animator;
    private EchoAi aiScript;
    public int directionToMove = 1; // 1 for forward, -1 for backward
    private Vector2 directionToLook;
    private bool LookOnce = false;

    void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
        animator = GetComponent<Animator>();
        aiScript = GetComponent<EchoAi>();
    }

    void Update()
    {
        Debug.Log("Direction to " + waypoints[waypointIndex].gameObject.name);
        if (aiScript.FaceAway == true)
        {
            Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
            // Calculate the direction from the trigger (this.transform.position) 
            // to the other object (other.transform.position)
            Vector2 directionToTarget = player.transform.position - this.transform.position;

            // Normalize the vector to get only the direction with a magnitude (length) of 1
            Vector2 normalizedDirection = directionToTarget.normalized;
            directionToLook = normalizedDirection;

            // You can now use normalizedDirection for various purposes
            //Debug.Log("Direction to " + collision.gameObject.name + ": " + normalizedDirection);

            if (normalizedDirection.x < -0.1f && directionToMove == 1)
            {
                Debug.Log("working?");
                if (LookOnce == false)
                {
                    // Resets to face forward when moving right.  Basically face right
                    transform.eulerAngles = new Vector2(0, 0);
                    LookOnce = true;
                }

                Move();
            }
            else if (normalizedDirection.x > 0.1f && directionToMove == -1)
            {
                if (LookOnce == false)
                {
                    // Sets the rotation exactly to 0, 180, 0.  Basically face left
                    transform.eulerAngles = new Vector2(0, 180);
                    LookOnce = true;
                }

                Move();
            }

            if (waypointIndex == 0)
            {
                Move();
            }
        }
       

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.3f)
        {
            IsMoving = false;
            LookOnce = false;
        }
        else
        {
            IsMoving = true;
        }

        if (IsMoving == false)
        {
            if (waypointIndex == waypoints.Length - 1)
            {
                directionToMove *= -1;
                waypointIndex = waypointIndex + directionToMove;
            }
            else
            {
                waypointIndex = waypointIndex + directionToMove;

                if (waypointIndex == 0)
                {
                    directionToMove *= -1; // Reverse the direction
                }
            }
        }

       
    }

    public void Move()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);

        //// Calculate the direction from the trigger (this.transform.position) 
        //// to the other object (other.transform.position)
        //Vector2 directionToTarget = waypoints[waypointIndex].transform.position - this.transform.position;

        //// Normalize the vector to get only the direction with a magnitude (length) of 1
        //Vector2 normalizedDirection = directionToTarget.normalized;

        //// You can now use normalizedDirection for various purposes
        Debug.Log("Direction to " + waypoints[waypointIndex].gameObject.name);

        //animator.SetFloat("DirX", normalizedDirection.x);
        //animator.SetFloat("DirY", normalizedDirection.y);

    }
}
