using UnityEngine;

public class RT_ArenaTriggerProxy : MonoBehaviour
{
    [Header("Reference to the boss AI script")]
    public RT_BossAI bossAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            bossAI.StartCutscene();
        }
    }
}
