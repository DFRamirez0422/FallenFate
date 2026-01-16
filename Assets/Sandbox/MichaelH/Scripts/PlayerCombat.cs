using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint; // Point from which the attack is measured
    public LayerMask enemyLayer; // Layer of enemies to hit
    [SerializeField] private bool show; // Show attack range in editor

    [Tooltip("Cooldown time between attacks in seconds.")]
    [SerializeField] private float cooldownTime = 1.5f;
    private float timer; // Timer to track cooldown
    
    // Attack parameters
    [Tooltip("Damage dealt per attack.")]
    [SerializeField] private int attackDamage = 1;
    [Tooltip("Range of the attack.")]
    [SerializeField] private float attackRange = 1f;
    
    // Knockback parameters
    [Tooltip("Force applied to enemies when hit.")]
    [SerializeField] private float knockbackForce = 50f;
    [Tooltip("Duration of knockback.")]
    [SerializeField] private float knockbackTime = 0.15f;
    
    [Tooltip("Duration of stun after knockback.")]
    [SerializeField] private float stunTime = .3f;

    private void Update()
    {
        // Update the timer for attack cooldown
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        // Check for attack input
        if (Input.GetButtonDown("Attack"))
        {
            PerformAttack();
        }
    }
    
    // --- Attack function to trigger attack animation ---
    public void PerformAttack()
    {
        if (timer > 0f) return; // still in cooldown
        
        if (timer <= 0)
        {
            // Play attack animation and set attacking state
            animator.SetBool("isAttacking", true);
            
            timer = cooldownTime;
        }
    }

    // --- Function called by animation event to deal damage ---
    public void DealDamage()
    {
        // Detect enemies in range of attack and damage them
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D col in enemies)
        {
            Health health = col.GetComponent<Health>();
            if (health == null) continue;

            health.Hit(attackDamage);
            Debug.Log("Hit enemy for " + attackDamage);
            
            // Apply knockback
            EnemyKnockback knockback = col.GetComponent<EnemyKnockback>();
            if (knockback == null) continue;
                
            knockback.Knockback(transform, knockbackForce, knockbackTime, stunTime);
                
            break; // hit only one enemy
        }
    }

    // --- Reset attack animation state ---
    public void FinishAttack()
    {
        animator.SetBool("isAttacking", false);
    }
    
    // --- Visualize attack range in editor ---
    private void OnDrawGizmosSelected()
    {
        if (!show || attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}