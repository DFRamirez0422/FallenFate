using UnityEngine;
using System.Collections.Generic;
using NPA_Health_Components;

public class EnemyHitboxController : MonoBehaviour
{
    [Tooltip("Data asset containing all hitbox definitions for this enemy")]
    public EnemyHitboxData hitboxData;
    
    [Tooltip("Optional reference point for hitbox origin")]
    public Transform attackPoint;
    
    [Tooltip("Manual test for triggering hitboxes (for development only)")]
    public KeyCode testKey = KeyCode.H;

    private bool isActive;                     // Tracks if a hitbox is currently active
    private float activeTimer;                 // Countdown timer for active hitbox duration
    private HitboxDefinition currentHitbox;    // Currently active hitbox definition
    private readonly List<Collider> alreadyHit = new(); // Keeps track of colliders already hit

    private void Update()
    {
        if (hitboxData == null) return;

        // Manual test trigger (used until animations are ready)
        if (Input.GetKeyDown(testKey))
        {
            string selectedID = GetHitboxIDByName(""); // this should fallback to first
            Debug.Log($"[DEBUG KEY] Trying to activate: {selectedID}");
            ActivateHitbox(selectedID);
        }

        // When a hitbox is active, update its lifetime and perform collision checks
        if (isActive)
        {
            activeTimer -= Time.deltaTime; // decrease timer each frame
            PerformHitDetection();         // check for collisions

            if (activeTimer <= 0f)         // deactivate when time runs out
                DeactivateHitbox();
        }
    }

    // Activates a hitbox by its ID name from the data asset
    public void ActivateHitbox(string attackId)
    {
        Debug.Log($"Attempting to activate hitbox: {attackId}");
        if (hitboxData == null) return;

        var def = hitboxData.hitboxes.Find(h => h.attackId == attackId);
        if (string.IsNullOrEmpty(def.attackId))
        {
            Debug.LogWarning($"Hitbox ID {attackId} was not found in {hitboxData.name}.");
            return;
        }

        currentHitbox = def;
        isActive = true;
        activeTimer = def.activeTime;
        alreadyHit.Clear(); // reset hit list for this activation
        
        Invoke(nameof(DeactivateHitbox), def.activeTime);

        if (def.bDebugDraw)
            Debug.Log($"Hitbox ID {def.attackId} is activated.");
    }

    // Deactivates the hitbox and clears any stored hits
    private void DeactivateHitbox()
    {
        isActive = false;

        if (currentHitbox.bDebugDraw)
            Debug.Log($"Hitbox ID {currentHitbox.attackId} is deactivated.");
    }

    // Performs overlap checks depending on hitbox shape
    private void PerformHitDetection()
    {
        Vector3 origin = attackPoint ? attackPoint.position : transform.position;
        origin += transform.TransformDirection(currentHitbox.offset);

        Collider[] hits = null;

        switch (currentHitbox.shape)
        {
            case HitboxShape.Box:
                hits = Physics.OverlapBox(origin, currentHitbox.size / 2f, transform.rotation, currentHitbox.targetMask);
                break;

            case HitboxShape.Sphere:
                hits = Physics.OverlapSphere(origin, currentHitbox.radius, currentHitbox.targetMask);
                hits = FilterByConeAngle(hits, origin); // optional angle filter for cone attacks
                break;
        }

        if (hits == null || hits.Length == 0) return;

        foreach (var hit in hits)
        {
            // Ignore targets already hit or out of 2.5D lane depth
            if (alreadyHit.Contains(hit)) continue;
            if (Mathf.Abs(hit.transform.position.z - transform.position.z) > currentHitbox.depthTolerance) continue;

            // Apply damage to valid targets with Health component
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(currentHitbox.damage);
                alreadyHit.Add(hit);
            }
        }
    }

    // Filters colliders by cone angle from forward direction
    private Collider[] FilterByConeAngle(Collider[] hits, Vector3 origin)
    {
        List<Collider> filtered = new();

        foreach (var hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(transform.forward, toTarget);

            if (angle <= currentHitbox.angle / 2f)
                filtered.Add(hit);
        }

        return filtered.ToArray();
    }
    
    // Retrieves hitbox ID by name, returns empty string if not found
    public string GetHitboxIDByName(string attackId)
    {
        // Guard clause: no data
        if (hitboxData == null || hitboxData.hitboxes.Count == 0)
            return string.Empty;

        // No name provided → return first entry (default)
        if (string.IsNullOrEmpty(attackId))
            return hitboxData.hitboxes[0].attackId;

        // Try to find the requested attack
        HitboxDefinition found = hitboxData.hitboxes.Find(h => h.attackId == attackId);

        // Return found or fallback to first
        return !string.IsNullOrEmpty(found.attackId)
            ? found.attackId
            : hitboxData.hitboxes[0].attackId;
    }

    // Draws hitbox gizmos in editor when selected (for debugging)
    private void OnDrawGizmos()
    {
        if (!isActive || !currentHitbox.bDebugDraw) return;

        Vector3 origin = attackPoint ? attackPoint.position : transform.position;
        origin += transform.TransformDirection(currentHitbox.offset);

        Gizmos.color = currentHitbox.debugColor.a > 0 ? currentHitbox.debugColor : Color.red;

        switch (currentHitbox.shape)
        {
            case HitboxShape.Box:
                Gizmos.DrawWireCube(origin, currentHitbox.size);    // size directly
                break;
            case HitboxShape.Sphere:
            case HitboxShape.Cone:                                  // preview as sphere for cone
                Gizmos.DrawWireSphere(origin, currentHitbox.radius);
                break;
        }
    }
}