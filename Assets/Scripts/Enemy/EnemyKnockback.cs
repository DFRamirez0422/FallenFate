using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    // ===== PRIVATE FIELDS ===== //
    private Rigidbody2D m_Rigidbody;
    private EnemyMovement m_EnemyMovement;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_EnemyMovement = GetComponent<EnemyMovement>();
    }

    public void Knockback(Transform player_transform, float knockback_force, float stun_time)
    {
        m_EnemyMovement.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(stun_time));
        Vector2 direction = (transform.position - player_transform.position).normalized;
        m_Rigidbody.linearVelocity = direction * knockback_force;
    }

    IEnumerator StunTimer(float stun_time)
    {
        yield return new WaitForSeconds(stun_time);
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_EnemyMovement.ChangeState(EnemyState.Idle);
    }
}
