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
    public PickUp_Manager pickupManager;
    public QuestManager questManager;

    private readonly List<GameObject> enemies = new List<GameObject>();
    private readonly List<GameObject> items = new List<GameObject>();

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
        enemies.Clear();
        items.Clear();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealthComponent = playerObj.GetComponent<PlayerHealth>();
        }

        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        items.AddRange(GameObject.FindGameObjectsWithTag("Item"));

        pickupManager = FindObjectOfType<PickUp_Manager>();
        questManager = FindObjectOfType<QuestManager>();
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

        // Edge case: never save a dead checkpoint
        data.playerHealth = Mathf.Max(currentHealth, 1);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeSelf)
                data.activeEnemies.Add(enemy.name);
        }

        foreach (GameObject item in items)
        {
            if (item != null && !item.activeSelf)
                data.collectedItems.Add(item.name);
        }

        if (questManager != null)
           // data.questState = questManager.CurrentQuestState;

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
            playerHealthComponent.m_CurrentHealth = Mathf.Max(data.playerHealth, 1);
            
        }

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.SetActive(data.activeEnemies.Contains(enemy.name));
        }

        foreach (GameObject item in items)
        {
            if (item == null) continue;
            bool wasCollected = data.collectedItems.Contains(item.name);
            item.SetActive(!wasCollected);
        }

      //if (questManager != null)
      // {
      //     questManager.CurrentQuestState = data.questState;
      //     questManager.ApplyQuestState();
      // }
    }
}