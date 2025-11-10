using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Hitstun_JoseE : MonoBehaviour
{
    [SerializeField] public float defaultStunDuration = 0.3f; 

    private float stunTimer = 0f;
    private bool isStunned = false;

    private Renderer rend;
    private Color originalColor;

    // vvvvv Added by Jose E. from original file. vvvvv //

    /// <summary>
    /// Expoed public variable that returns whether or not the current entity is in a hit stun.
    /// </summary>
    //public bool IsStunned => isStunned;

    /// Simple code to check if hitstun should be applied for animator. I Only wish
    /// I knew which module or function controlled whether or not the player
    /// was hit so I could try to integrate into the animator controller.
    private void ProcessWhenEnemyNear()
    {
        if (!IsStunned)
        {
            var colliders = Physics.OverlapSphere(transform.position, 1.0f);
            foreach (Collider collider in colliders)
            {
                EnemyHitboxController enemy_hitbox;
                if (collider.gameObject.TryGetComponent(out enemy_hitbox))
                {
                    Debug.Log("Animator::: Hitstune!");
                    ApplyHitstun(1.0f);
                }
            }
        }
    }

    // ^^^^^ Added by Jose E. from original file. ^^^^^ //

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
                EndStun();

        }

        // ADDED BY: Jose E.
        ProcessWhenEnemyNear();
    }

    public void ApplyHitstun(float duration)
    {
        isStunned = true;
        stunTimer = duration > 0 ? duration : defaultStunDuration;

        // Flash red
        if (rend != null)
            rend.material.color = Color.red;

        // Disable enemy attacking (AI can check IsStunned)
        // Example: enemyAI.SetCanAttack(false);
    }

    private void EndStun()
    {
        isStunned = false;

        // Restore visuals
        if (rend != null)
            rend.material.color = originalColor;

        // Re-enable AI attacking
        // Example: enemyAI.SetCanAttack(true);
    }

    public bool IsStunned => isStunned;
}