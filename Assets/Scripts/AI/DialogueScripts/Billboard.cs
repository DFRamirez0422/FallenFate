using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Get direction from camera to object
        Vector3 direction = mainCamera.transform.position - transform.position;

        // Keep the text upright
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            // Look at the camera
            transform.rotation = Quaternion.LookRotation(direction);

            // Ensure the text faces forward (readable)
            transform.Rotate(0f, 180f, 0f);
        }
    }
}