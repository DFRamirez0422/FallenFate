using UnityEngine;

public class DisableHitEffect : MonoBehaviour
{
    /// <summary>Call from animation event or timer to disable this hit effect.</summary>
    public void DisableEffect()
    {
        gameObject.SetActive(false);
    }
}
