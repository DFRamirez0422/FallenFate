using UnityEngine;

public class PulseOnly : MonoBehaviour
{
    [SerializeField] private float pulseSize = 1.15f;   // The size of the pulse
    [SerializeField] private float returnSpeed = 5f;   // The speed of the return
    private Vector3 startSize; // Original scale of UI

    private void Start()
    {
        startSize = transform.localScale;
        // Don't set to zero - start at normal size so it's visible
        // If you need it to start invisible, handle that separately
    }

    private void Update()
    {
        // Lerp back to startSize if we're currently larger than it (after a pulse)
        if (transform.localScale.magnitude > startSize.magnitude * 1.01f) // Small threshold to avoid jitter
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
        }
    }
    
    public void Pulse()
    {
        // Set scale to pulse size - Update will lerp it back down
        transform.localScale = startSize * pulseSize;
    }
}
