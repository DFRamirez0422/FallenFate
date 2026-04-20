using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carries player health across scenes. Must be a singleton: a second StatTracker in a loaded scene
/// would start with lastPlayerHealth == 0 and, on sceneLoaded, treat the player as "not alive" and
/// force full health (see ApplyCarriedHealthToPlayer).
/// </summary>
public class StatTracker : MonoBehaviour
{
    private static StatTracker s_Instance;

    private int lastPlayerHealth;

    public bool IsAlive => lastPlayerHealth > 0;

    private PlayerHealth PlayerObject;

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Debug.LogWarning(
                "[StatTracker] Destroying duplicate instance. Only one StatTracker may exist (singleton); " +
                "a second one starts with carried health 0 and can overwrite the player to full HP on load. " +
                $"Remove extra StatTracker/HealthTracker objects from this scene: '{gameObject.name}' in '{gameObject.scene.name}'. " +
                "Run: Tools → Fallen Fate → Validate StatTracker In Build Scenes.",
                this);
            Destroy(gameObject);
            return;
        }

        s_Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (s_Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            s_Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(SyncPlayerHealthAfterSceneLoad());
    }

    private IEnumerator SyncPlayerHealthAfterSceneLoad()
    {
        yield return null;

        PlayerObject = ResolvePlayerHealth();
        ApplyCarriedHealthToPlayer();
    }

    private PlayerHealth ResolvePlayerHealth()
    {
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            PlayerHealth onRoot = playerGo.GetComponent<PlayerHealth>();
            if (onRoot != null)
                return onRoot;
        }

        return FindObjectOfType<PlayerHealth>();
    }

    private void ApplyCarriedHealthToPlayer()
    {
        if (PlayerObject == null)
            return;

        if (IsAlive)
            PlayerObject.m_CurrentHealth = lastPlayerHealth;
        else
            PlayerObject.m_CurrentHealth = PlayerObject.m_MaxHealth;
    }

    private void Update()
    {
        if (PlayerObject != null)
            lastPlayerHealth = PlayerObject.CurrentHealth;
    }

    /// <summary>
    /// Pushes the current health value into carry-over storage immediately.
    /// Call this whenever <see cref="PlayerHealth"/> changes HP so a same-frame
    /// scene load (e.g. death → LoadGameOver) cannot leave a stale lastPlayerHealth
    /// (e.g. still 1 after a 1→0 kill) before <see cref="Update"/> runs.
    /// </summary>
    public static void PublishPlayerHealth(int health)
    {
        if (s_Instance != null)
            s_Instance.lastPlayerHealth = health;
    }
}
