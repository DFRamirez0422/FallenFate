using System.Collections.Generic;
using UnityEngine;

//Updated to manage picked up items using a list of Item_Data ScriptableObjects
// Manages the list of picked up items
public class PickUp_Manager : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public List<Item_Data> items;

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
