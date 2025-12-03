using UnityEngine;

public class PaperMarioCamera : MonoBehaviour
{
    public Transform target;              // Player to follow

    [Header("Default Camera Settings")]
    public float distance = 8f;           // Normal follow distance
    public float height = 6f;             // Normal height
    public Vector3 angle = new Vector3(45f, 0f, 0f); // Camera tilt (angled view)

    [Header("Zoom Settings")]
    public float zoomDistance = 12f;      // Distance when zoomed out
    public float zoomHeight = 9f;         // Height when zoomed out
    public float smoothSpeed = 5f;        // Smoothness of movement

    private float targetDistance;         // Current goal distance
    private float targetHeight;           // Current goal height

    void Start()
    {
        // Start at normal values
        targetDistance = distance;
        targetHeight = height;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Lerp towards target zoom/height values
        float currentDistance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * smoothSpeed);
        float currentHeight = Mathf.Lerp(height, targetHeight, Time.deltaTime * smoothSpeed);

        // Apply updated values
        distance = currentDistance;
        height = currentHeight;

        // Desired position
        Vector3 offset = new Vector3(0, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        // Smooth follow movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Fixed rotation (angled view)
        transform.rotation = Quaternion.Euler(angle);
    }

    // Called by trigger zones
    public void SetZoom(bool zoomOut)
    {
        if (zoomOut)
        {
            targetDistance = zoomDistance;
            targetHeight = zoomHeight;
        }
        else
        {
            targetDistance = 8f; // Reset to defaults
            targetHeight = 6f;
        }
    }
}

