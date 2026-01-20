using System.Collections;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    // ===== PRIVATE FIELDS ===== //
    private Rigidbody2D m_Rigidbody;
    private EnemyMovement m_EnemyMovement;
    private Coroutine m_StunRoutine;
    
    public float KnockbackTime { get; private set; }
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_EnemyMovement = GetComponent<EnemyMovement>();
    }

    public void Knockback(Transform playerTransform, float knockbackForce, float knockbackTime, float stunTime)    
    {
        m_EnemyMovement.ChangeState(EnemyState.Knockback);

        KnockbackTime = knockbackTime;
        
        Vector2 direction = (transform.position - playerTransform.position).normalized;

        // Apply knockback velocity
        m_Rigidbody.linearVelocity = direction * knockbackForce;

        // Restart stun coroutine
        if (m_StunRoutine != null)
            StopCoroutine(m_StunRoutine);

        m_StunRoutine = StartCoroutine(StunTime(knockbackTime, stunTime));;
    }

    private IEnumerator StunTime(float knockbackTime, float stunTime)
    {
        // Wait for knockback velocity to play out
        yield return new WaitForSeconds(knockbackTime);
        
        // Stop knockback velocity
        m_Rigidbody.linearVelocity = Vector2.zero;
        
        // Now stun the enemy (prevents AI but doesn't affect physics)
        if (m_EnemyMovement != null)
        {
            m_EnemyMovement.SetStunned(true);
        }
        
        // Wait for stun duration
        yield return new WaitForSeconds(stunTime);
        
        // Unstun and return to idle
        if (m_EnemyMovement != null)
        {
            m_EnemyMovement.SetStunned(false);
            m_EnemyMovement.ChangeState(EnemyState.Idle);
        }
        m_StunRoutine = null;
    }    
}
