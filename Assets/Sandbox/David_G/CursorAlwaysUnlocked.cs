using UnityEngine;

/// <summary>
/// Persists across scenes and forces the cursor visible and unlocked every frame.
/// </summary>
public class CursorAlwaysUnlocked : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
