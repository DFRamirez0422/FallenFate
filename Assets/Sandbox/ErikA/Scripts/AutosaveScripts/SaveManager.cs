using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [Header("Runtime References")]
    public Transform player;
    public PlayerHealth playerHealthComponent;
    
    private bool applyingLoad = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CacheSceneObjects();
    }

    public void CacheSceneObjects()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealthComponent = playerObj.GetComponent<PlayerHealth>();
        }
    }

    public void SaveGame()
    {
        CacheSceneObjects();

        if (player == null)
        {
            Debug.LogWarning("Save failed: Player not found.");
            return;
        }

        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        data.playerPosition[0] = player.position.x;
        data.playerPosition[1] = player.position.y;
        data.playerPosition[2] = player.position.z;

        int currentHealth = 1;
        if (playerHealthComponent != null)
            currentHealth = playerHealthComponent.CurrentHealth;

        // Never persist "dead" as 1 HP — that makes the next load always start at 1.
        // If we are saving while dead, store full health so respawn / load is playable.
        if (currentHealth <= 0 && playerHealthComponent != null)
            currentHealth = playerHealthComponent.MaxHealth;
        else
            currentHealth = Mathf.Max(currentHealth, 1);

        data.playerHealth = currentHealth;

        SaveSystem.Save(data);
        Debug.Log("Autosave written.");
    }

    public void LoadLastSave()
    {
        SaveData data = SaveSystem.Load();
        if (data == null)
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        StartCoroutine(LoadGameRoutine(data));
    }

    public void LoadGame(SaveData data)
    {
        if (data == null)
            return;

        StartCoroutine(LoadGameRoutine(data));
    }

    private IEnumerator LoadGameRoutine(SaveData data)
    {
        if (applyingLoad)
            yield break;

        applyingLoad = true;

        Time.timeScale = 1f;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(data.sceneName);
        while (!loadOp.isDone)
            yield return null;

        yield return null;

        CacheSceneObjects();
        ApplySaveData(data);

        applyingLoad = false;
    }

    private void ApplySaveData(SaveData data)
    {
        if (player != null)
        {
            player.position = new Vector3(
                data.playerPosition[0],
                data.playerPosition[1],
                data.playerPosition[2]
            );
        }

        if (playerHealthComponent != null)
        {
            int loaded = data.playerHealth <= 0 ? playerHealthComponent.MaxHealth : data.playerHealth;
            playerHealthComponent.m_CurrentHealth = Mathf.Clamp(loaded, 1, playerHealthComponent.MaxHealth);
            StatTracker.PublishPlayerHealth(playerHealthComponent.m_CurrentHealth);
        }
    }
}