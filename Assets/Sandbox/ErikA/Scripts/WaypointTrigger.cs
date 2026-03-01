using System;
using UnityEngine;


public class WaypointTrigger : MonoBehaviour
{
    [SerializeField] CameraWaypointSystem cameraWaypointSystem;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        cameraWaypointSystem.StartWaypointSequence();
        Debug.Log("Waypoint Sequence Complete");
    }

    void OnTriggerExit2D(Collider2D other)
    {
         gameObject.SetActive(false);
    }
    
}
