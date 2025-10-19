using UnityEngine;

public class BossThrower : MonoBehaviour
{
    // References for the boss projectiles
    public GameObject puddleProjectilePrefab;
    public GameObject explosiveProjectilePrefab;

    // Where and how the boss throws projectiles
    public Transform firePoint;
    public Transform player;
    public float throwInterval = 3f; // Time between each attack

    private float throwTimer = 0f; // Keeps track of when to throw next

    void Update()
    {
        // If there's no player assigned, do nothing
        if (player == null) return;

        // Make the boss face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Keeps boss from tilting up or down
        transform.forward = direction;

        // Increase timer every frame
        throwTimer += Time.deltaTime;

        // When timer reaches the throw interval, fire a projectile
        if (throwTimer >= throwInterval)
        {
            ThrowRandomProjectile();
            throwTimer = 0f;
        }
    }

    void ThrowRandomProjectile()
    {
        // Check if everything is assigned correctly in the Inspector
        if (puddleProjectilePrefab == null || explosiveProjectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("BossThrower is missing references!");
            return;
        }

        // Randomly pick between puddle and explosive projectiles
        GameObject chosenProjectile = Random.value > 0.5f ? puddleProjectilePrefab : explosiveProjectilePrefab;

        // Spawn the chosen projectile at the fire point
        Instantiate(chosenProjectile, firePoint.position, firePoint.rotation);
    }
}
