using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public Transform[] waypoints;

    [SerializeField]
    float moveSpeed = 2f;

    [HideInInspector]
    public int waypointIndex = 0;

    void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);
    }
}
