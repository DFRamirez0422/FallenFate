using UnityEngine;

public class ParabolicPowerUpMovement : MonoBehaviour
{
    private Vector3 startPosition;
    private Transform playerTarget;
    private float parabolaHeight;
    private float flightTime;
    private float currentTime = 0f;
    private bool isInitialized = false;
    
    public void Initialize(Vector3 start, Transform player, float height, float duration)
    {
        startPosition = start;
        playerTarget = player;
        parabolaHeight = height;
        flightTime = duration;
        currentTime = 0f;
        isInitialized = true;
        
        // Disable physics/rigidbody so we can control movement directly
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    void Update()
    {
        if (!isInitialized || playerTarget == null)
        {
            // Try to find player if not set
            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
            }
            return;
        }
        
        // Update time progress
        currentTime += Time.deltaTime;
        float t = Mathf.Clamp01(currentTime / flightTime);
        
        // Continuously update position along parabola, tracking player's current position
        Vector3 currentEndPosition = playerTarget.position;
        Vector3 parabolicPosition = MathParabola.Parabola(startPosition, currentEndPosition, parabolaHeight, t);
        
        // Set position directly
        transform.position = parabolicPosition;
        
        // If parabola is complete (t >= 1), move directly toward player
        if (t >= 1f)
        {
            Vector3 directionToPlayer = (currentEndPosition - transform.position).normalized;
            float moveSpeed = 5f; // Speed when following player after parabola completes
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        }
    }
}

