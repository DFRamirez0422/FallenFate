using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Updated to manage picked up items using a list of Item_Data ScriptableObjects
// Manages the list of picked up items
public class PickUp_Manager : MonoBehaviour
{
    public static PickUp_Manager Instance { get; private set; }

    static string s_PreviousSceneName;

    public List<Item_Data> items;

    void Awake()
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;

        if (string.IsNullOrEmpty(s_PreviousSceneName))
        {
            s_PreviousSceneName = name;
            return;
        }

        bool atMainMenu = name == "MainMenu";
        bool reloadSameLevel = s_PreviousSceneName == name;
        bool continuedAfterGameOver = s_PreviousSceneName == "GameOver_NEW";

        if (atMainMenu || reloadSameLevel || continuedAfterGameOver)
            ClearKeyItems();

        s_PreviousSceneName = name;
    }

    void ClearKeyItems()
    {
        if (items == null)
            return;

        items.RemoveAll(i => i != null && i.pickupType == Item_Data.PickUpType.keys);
    }

    public int GetAmountOfItem(Item_Data item)
    {
        return items.FindAll(i => i == item).Count;
    }

    public List<LocationSO> discoveredLocations = new List<LocationSO>();

    public void recordLocationDiscovery(LocationSO location)
    {
        if (!discoveredLocations.Contains(location))
        {
            discoveredLocations.Add(location);
            Debug.Log($"New location discovered: {location.locationName}");
        }
    }
    
    public bool HasDiscoveredLocation(LocationSO location)
    {
        return discoveredLocations.Contains(location);
    }

}
