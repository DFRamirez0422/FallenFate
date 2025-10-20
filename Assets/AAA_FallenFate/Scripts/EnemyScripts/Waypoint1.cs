using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Waypoint : MonoBehaviour
{
    public List<Transform> waypoint;
    public Transform self;

    public int currentWaypointIndex = 0;

    public void Walking()
    {
        if (waypoint.Count == 0)
        {
            Debug.Log("No waypoints assigned");
            return;
        }

        float distanceToWaypoint = Vector3.Distance(waypoint[currentWaypointIndex].position, self.transform.position);

        if (distanceToWaypoint <= 2)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoint.Count;
        }
    }
}

