using UnityEngine;

public class DisableHitEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void DisableEffect()
    {
        gameObject.SetActive(false);
    }
}
