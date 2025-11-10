using UnityEngine;

public class ArenaTriggerProxy : MonoBehaviour
{
    [Header("Reference to the boss AI script")]
    public BossAI bossAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            bossAI.StartCutscene();
        }
    }
}
