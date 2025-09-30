using UnityEngine;

public class Hitstun : MonoBehaviour
{
    [SerializeField] public float defaultStunDuration = 0.3f; 

    private float stunTimer = 0f;
    private bool isStunned = false;

    private Renderer rend;
    private Color originalColor;

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