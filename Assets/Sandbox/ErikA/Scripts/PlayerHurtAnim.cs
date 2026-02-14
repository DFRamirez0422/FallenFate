using UnityEngine;

public class PlayerHurtAnim : MonoBehaviour
{
    private Animator anim;
    private PlayerHealth health;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
    }

    public void PlayHurt()
    {
        var src = health.LastHitSource;
        if (src == null) return;

        Vector3 dirEnemy = (src.position - transform.position);
        dirEnemy.z = 0f;         // correct for 2D XY
        dirEnemy.Normalize();

        // If your player rotates in 2D, this keeps left/right correct relative to player
        Vector3 dirLocal = transform.InverseTransformDirection(dirEnemy);

        anim.SetFloat("HitDirX", Mathf.Clamp(dirLocal.x, -1f, 1f));
        anim.SetFloat("HitDirY", Mathf.Clamp(dirLocal.y, -1f, 1f));
        anim.SetTrigger("Hurt");
    }
}