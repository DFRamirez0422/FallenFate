using UnityEngine;

public class LocationSoTrigger : MonoBehaviour
{
    // This script is responsible for triggering the discovery of a location when the player enters a specific area.
  [SerializeField] private LocationSO location;
  [SerializeField] private LocationSO returnLocation;
  [SerializeField] private Item_Data itemNeeded;

  [SerializeField] private PickUp_Manager pickUpManager;

  void Start()
  {
    // This script is responsible for triggering the discovery of a location when the player enters a specific area.
     pickUpManager = GameObject.FindGameObjectWithTag("PickUp_Manager").GetComponent<PickUp_Manager>();
  }

    //Adds LocationSO to list of discovered locations in PickUp_Manager
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !pickUpManager.discoveredLocations.Contains(location) && location != null)
        {
            pickUpManager.recordLocationDiscovery(location);
        }
        else if(other.CompareTag("Player") && pickUpManager.items.Contains(itemNeeded) && returnLocation != null)
        {
            pickUpManager.recordLocationDiscovery(returnLocation);
        }
    }
}
