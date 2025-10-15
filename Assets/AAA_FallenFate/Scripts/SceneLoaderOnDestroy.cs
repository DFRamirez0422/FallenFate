using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Loads a scene when another collider enters this object's trigger.
// Requirements: This GameObject must have a Collider with "Is Trigger" enabled.
// Either this object or the entering object should have a Rigidbody.
public class SceneLoaderOnDestroy : MonoBehaviour
{
    [Header("Scene Loading Settings")]
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private float delayBeforeLoad = 0f;
    [SerializeField] private string requiredTag = "Player"; // Only this tag can trigger (leave empty for any)

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool hasTriggered = false;

    private void Reset()
    {
        // Try to auto-set trigger on the collider if present (Editor-time convenience)
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        TryStartLoadFlow($"Trigger entered by '{other.name}'. Loading scene: {sceneToLoad}");
    }

    // Public method to manually trigger scene load (useful for testing & UI)
    public void LoadScene()
    {
        TryStartLoadFlow($"Manual trigger called. Loading scene: {sceneToLoad}");
    }

    // Public method to set the scene to load at runtime
    public void SetSceneToLoad(string sceneName)
    {
        sceneToLoad = sceneName;
    }

    private void TryStartLoadFlow(string debugMessage)
    {
        if (hasTriggered) return;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            if (showDebugLogs)
                Debug.LogWarning($"SceneLoaderOnDestroy on '{gameObject.name}' has no scene to load!");
            return;
        }

        hasTriggered = true;
        if (showDebugLogs && !string.IsNullOrEmpty(debugMessage))
            Debug.Log(debugMessage);

        if (delayBeforeLoad > 0f)
        {
            StartCoroutine(LoadSceneAfterDelay());
        }
        else
        {
            LoadSceneImmediately();
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        LoadSceneImmediately();
    }

    private void LoadSceneImmediately()
    {
        if (showDebugLogs)
            Debug.Log($"Loading scene: {sceneToLoad}");

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
