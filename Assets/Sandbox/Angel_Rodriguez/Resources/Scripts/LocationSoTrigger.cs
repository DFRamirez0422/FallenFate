using UnityEngine;

public class LocationSoTrigger : MonoBehaviour
{
    // This script is responsible for triggering the discovery of a location when the player enters a specific area.
  [SerializeField] private LocationSO location;
    [SerializeField] private PickUp_Manager pickUpManager;


  [Header("Optional Settings")]
   [Tooltip("If the player has this item, it will trigger the discovery of the returnLocation instead of the location above.")]
  [SerializeField] private LocationSO returnLocation;
  [SerializeField] private Item_Data itemNeeded;

  void Start()
  {
    // This script is responsible for triggering the discovery of a location when the player enters a specific area.
     pickUpManager = GameObject.FindGameObjectWithTag("PickUp_Manager").GetComponent<PickUp_Manager>();
  }

    //Adds LocationSO to list of discovered locations in PickUp_Manager
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !pickUpManager.discoveredLocations.Contains(location))
        {
            pickUpManager.recordLocationDiscovery(location);
        }
        else if(other.CompareTag("Player") && pickUpManager.items.Contains(itemNeeded) && itemNeeded != null)
        {
            pickUpManager.recordLocationDiscovery(returnLocation);
        }
    }
}
