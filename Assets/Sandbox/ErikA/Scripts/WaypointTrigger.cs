using System;
using UnityEngine;


public class WaypointTrigger : MonoBehaviour
{
    [SerializeField] CameraWaypointSystem cameraWaypointSystem;
    [SerializeField] string triggeringTag = "Player";

    private bool m_HasTriggered;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_HasTriggered) return;
        if (!other.CompareTag(triggeringTag)) return;

        if (cameraWaypointSystem == null) {
            Debug.LogWarning("WaypointTrigger has no CameraWaypointSystem assigned.", gameObject);
            return;
        }

        m_HasTriggered = true;
        cameraWaypointSystem.StartWaypointSequence();
        Debug.Log("Waypoint sequence started.");
    }

    void OnTriggerExit2D(Collider2D other)
    {
         if (!m_HasTriggered) return;
         if (!other.CompareTag(triggeringTag)) return;
         gameObject.SetActive(false);
    }
    
}
