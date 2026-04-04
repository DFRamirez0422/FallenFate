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
    private int direction = 1; // 1 for forward, -1 for backward

    void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.3f)
        {
            IsMoving = false;
        }
        else
        {
            IsMoving = true;
        }

        if (IsMoving == false)
        {
            waypointIndex = waypointIndex + direction;

            if (waypointIndex == waypoints.Length - 1 || waypointIndex == 0)
            {
                direction *= -1; // Reverse the direction
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
