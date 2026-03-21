using UnityEngine;
using UnityEngine.SceneManagement;

public class StatTracker : MonoBehaviour
{
    private int lastPlayerHealth;
    private Scene currentScene;
    

    [SerializeField] private PlayerHealth PlayerObject;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        PlayerStats();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the new player in the scene
        PlayerObject = FindObjectOfType<PlayerHealth>();

        if (PlayerObject != null)
        {
            PlayerObject.m_CurrentHealth = lastPlayerHealth;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void PlayerStats()
    {
        if (PlayerObject != null)
        {
            lastPlayerHealth = PlayerObject.CurrentHealth;
        }
    }
}