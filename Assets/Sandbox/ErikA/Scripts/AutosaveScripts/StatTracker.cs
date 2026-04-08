using UnityEngine;
using UnityEngine.SceneManagement;

public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance;
    
    private int lastPlayerHealth;
    private Scene currentScene;
    
    public bool IsAlive => lastPlayerHealth > 0;

    [SerializeField] private PlayerHealth PlayerObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        if (PlayerObject == null)
            return;
        
        if (PlayerObject != null && IsAlive)
        {
            PlayerObject.m_CurrentHealth = lastPlayerHealth;
        }
        else if (!IsAlive)
        {
            PlayerObject.m_CurrentHealth = PlayerObject.m_MaxHealth;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
          SceneManager.sceneLoaded -= OnSceneLoaded;  
        }
        
    }

    private void PlayerStats()
    {
        if (PlayerObject != null)
        {
            lastPlayerHealth = PlayerObject.CurrentHealth;
        }
    }
}