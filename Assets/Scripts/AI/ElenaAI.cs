using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ElenaAI : MonoBehaviour
{
    private Transform player;
    private Transform Elena;
    private NavMeshAgent agent;

    public int followDistance;
    public bool CombatToggle = false;
    public GameObject[] objects;
    
    [Header("Throw_PowerUp + UI Settings")]
    public int PowerUpHold = 0;
    public GameObject PowerUp;
    public List<GameObject> PowerUpsInGame = new List<GameObject>();
    public GameObject PowerUpIcon;
    public GameObject BackgroundIcon;
    public Text BButtonText;
    [SerializeField] private Sprite NoPowerUpIcon;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        // Initialize UI for throw
        BButtonText.color = Color.midnightBlue;
        BackgroundIcon.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").transform;;


        Elena = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    void Start()
    {
        objects = GameObject.FindGameObjectsWithTag("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if (PowerUpsInGame.Count != 0 && PowerUpHold == 0 && CombatToggle ||
            PowerUpsInGame.Count != 0 && PowerUpHold == 0 && !CombatToggle)
        {
            RetrivePowerUp();
        }

        else if (!CombatToggle && PowerUpHold == 1 || PowerUpsInGame.Count == 0 && !CombatToggle){ FollowPlayer(); }

        else if (CombatToggle && PowerUpHold == 1 || PowerUpsInGame.Count == 0 && CombatToggle) { TakeCover(); }

        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0,0,0);

    }


    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, Elena.transform.position);

        if (player != null)
        {
            if (distanceToPlayer > followDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(Elena.position);
                return;
            }
        }
    }

    private void TakeCover()
    {
        GameObject closest = null;
        float minDistSq = float.MaxValue;
        Vector3 tPos = Elena.position;

        foreach (var obj in objects)
        {
            float distSq = (obj.transform.position - tPos).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closest = obj;
            }
        }

        agent.SetDestination(closest.transform.position);
    }

   //---- Change made by Angel.Rodriguez ----//
   // Retrieve the nearest power-up from the ground
   public void RetrivePowerUp()
   {
        GameObject  NearestPowerUp = null;
        float minDistSq = float.MaxValue;
        Vector3 ElenaPosition = Elena.position;

        foreach (var _PowerUp in PowerUpsInGame)
        {
            float distSq = (_PowerUp.transform.position - ElenaPosition).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                NearestPowerUp = _PowerUp;
            }
        }

        agent.SetDestination(NearestPowerUp.transform.position);
   }

    // Throw the held power-up towards the player
    public void ThrowPowerUp()
    {
        GameObject PowerHealth = null;
        PowerUpIcon.GetComponent<Image>().sprite = NoPowerUpIcon;
        BButtonText.color = Color.midnightBlue;
        Vector3 SpawnPosition = transform.position + transform.forward * 2f + Vector3.up * 1f;
        PowerHealth = Instantiate(PowerUp, SpawnPosition, Quaternion.identity);
        
        // Calculate direction from spawn position to player position
        Vector3 targetPosition = player.position;
        Vector3 displacement = targetPosition - SpawnPosition;
        float horizontalDistance = new Vector3(displacement.x, 0f, displacement.z).magnitude;
        float verticalDistance = displacement.y;
        
        // Normalize horizontal direction
        Vector3 horizontalDirection = new Vector3(displacement.x, 0f, displacement.z).normalized;
        if (horizontalDirection.magnitude < 0.1f) horizontalDirection = Vector3.forward; // Fallback
        
        // Calculate accurate ballistic trajectory using physics
        float gravity = Mathf.Abs(Physics.gravity.y);
        
        // Choose optimal launch angle based on distance
        // Longer distances need higher angles for accuracy
        float launchAngle = 45f * Mathf.Deg2Rad; // Default 45 degrees
        
        if (horizontalDistance > 15f)
        {
            launchAngle = 55f * Mathf.Deg2Rad; // High angle for long distance
        }
        else if (horizontalDistance > 8f)
        {
            launchAngle = 50f * Mathf.Deg2Rad; // Medium-high for medium distance
        }
        else if (horizontalDistance < 4f)
        {
            launchAngle = 40f * Mathf.Deg2Rad; // Lower angle for close range
        }
        
        // Adjust for height difference
        if (verticalDistance > 2f)
        {
            launchAngle += 8f * Mathf.Deg2Rad; // Throw higher if target is above
        }
        else if (verticalDistance < -2f)
        {
            launchAngle -= 5f * Mathf.Deg2Rad; // Throw lower if target is below
        }
        
        launchAngle = Mathf.Clamp(launchAngle, 35f * Mathf.Deg2Rad, 65f * Mathf.Deg2Rad);
        
        float cosAngle = Mathf.Cos(launchAngle);
        float sinAngle = Mathf.Sin(launchAngle);
        
        // Calculate required initial speed using ballistic trajectory formula
        // Formula: v = sqrt(g * x² / (2 * cos²(θ) * (x*tan(θ) - y)))
        // Where x = horizontalDistance, y = verticalDistance, θ = launchAngle
        
        float tanAngle = Mathf.Tan(launchAngle);
        float numerator = gravity * horizontalDistance * horizontalDistance;
        float denominator = 2f * cosAngle * cosAngle * (horizontalDistance * tanAngle - verticalDistance);
        
        float horizontalSpeed;
        float verticalSpeed;
        
        // Handle edge cases where formula might fail
        if (denominator <= 0.01f || horizontalDistance < 0.1f || Mathf.Abs(denominator) < 0.01f)
        {
            // Fallback: use time-based calculation
            float flightTime = Mathf.Sqrt(horizontalDistance / 6f);
            flightTime = Mathf.Clamp(flightTime, 0.6f, 2.5f);
            
            horizontalSpeed = horizontalDistance / flightTime;
            verticalSpeed = (verticalDistance + 0.5f * gravity * flightTime * flightTime) / flightTime;
        }
        else
        {
            // Use ballistic formula for accurate trajectory
            float initialSpeed = Mathf.Sqrt(numerator / denominator);
            
            // Clamp speed to reasonable values
            initialSpeed = Mathf.Clamp(initialSpeed, 7f, 25f);
            
            // Calculate velocity components
            horizontalSpeed = initialSpeed * cosAngle;
            verticalSpeed = initialSpeed * sinAngle;
        }
        
        // Ensure minimum speeds for accuracy at all distances
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, 5f, 20f);
        verticalSpeed = Mathf.Clamp(verticalSpeed, 4f, 16f);
        
        // Scale speeds slightly for very long distances to improve accuracy
        if (horizontalDistance > 12f)
        {
            horizontalSpeed *= 1.1f; // Slightly faster for long distance
            verticalSpeed *= 1.05f;
        }
        
        Rigidbody rb = PowerHealth.GetComponent<Rigidbody>();
        Vector3 throwVelocity = horizontalDirection * horizontalSpeed + Vector3.up * verticalSpeed;
        rb.linearVelocity = throwVelocity;
        
        // Calculate total throw speed for duration scaling
        float totalThrowSpeed = throwVelocity.magnitude;
        
        // Pass player reference to power-up for homing
        HealthPowerUP powerUpScript = PowerHealth.GetComponent<HealthPowerUP>();
        if (powerUpScript != null && player != null)
        {
            // Use reflection or add a public method to set target
            // For now, the HealthPowerUP will find the player automatically
        }
        
        // Lock player movement - duration scales with throw speed
        // Faster throws = longer lock duration (more time to catch it)
        if (player != null)
        {
            NPA_PlayerPrefab.Scripts.PlayerController playerController = player.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
            if (playerController != null)
            {
                // Scale duration: base 0.8s, scales from 0.5s (slow) to 1.5s (fast)
                // Speed range: ~7 (slow) to ~25 (fast)
                float speedNormalized = Mathf.InverseLerp(7f, 25f, totalThrowSpeed);
                float lockDuration = Mathf.Lerp(0.5f, 1.5f, speedNormalized);
                lockDuration = Mathf.Clamp(lockDuration, 0.5f, 1.5f);
                
                playerController.LockMovementForThrow(lockDuration);
            }
        }
    }

}
