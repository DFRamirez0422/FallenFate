using System.Linq.Expressions;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//I Have updated this script to use CollidableObject as a base class for consistency and better collision handling.

public class PickUpObjects : CollidableObject // Inherits from CollidableObject
{
    private PickUp_Manager pickUpManager;
    [SerializeField] private Item_Data itemData;
    [SerializeField] private GameObject PickUpPrompt;
    

    protected override void Start()
    {
        // Check if PickUpPrompt is assigned before using it
        if(PickUpPrompt != null)
        {
            PickUpPrompt.SetActive(false);
        }
        
        base.Start(); // Calls the Start method of CollidableObject and allows to be overridden by PickUpObjects Script
        
        // Check if Item_Data is assigned in Inspector
        if(itemData == null)
        {
            Debug.LogError("Item_Data is not assigned in Inspector for " + gameObject.name + ". Please assign it in the Inspector.");
            return; // Exit early if itemData is not assigned
        }

        // Find the Item_PickUp_Manager GameObject and check if it exists
        GameObject managerObject = GameObject.Find("Item_PickUp_Manager");
        if(managerObject != null)
        {
            pickUpManager = managerObject.GetComponent<PickUp_Manager>();
            if(pickUpManager == null)
            {
                Debug.LogError("Item_PickUp_Manager GameObject found but PickUp_Manager component is missing.");
                return;
            }
        }
        else
        {
            Debug.LogError("Item_PickUp_Manager GameObject not found in the scene. Please add it to the scene.");
            return;
        }

        // Check if item is already collected
        if (pickUpManager.items.Contains(itemData)){
            Debug.Log("Item " + itemData.itemName + " is already in PickUp_Manager");
            var copy = this.gameObject;
            Destroy(copy);
        }
        else
        {
            itemData.collected = false;
            Debug.Log("Item " + itemData.itemName + " is not in PickUp_Manager");
        }
    }

    // Override the OnCollide method to implement pick-up logic
    protected override void OnCollide(GameObject other)
    {
            if(Input.GetButtonDown("Interact"))
            {
                Debug.Log("Picked up " + gameObject.name);
                if (itemData != null && pickUpManager != null)
                {
                        itemData.collected = true;
                        pickUpManager.items.Add(itemData);
                        Debug.Log("Added " + itemData.itemName + " to PickUp_Manager");
                        var copy = this.gameObject;
                        Destroy(copy);
                }
                else
                {
                    if(itemData == null)
                        Debug.LogError("Item_Data is not assigned for " + gameObject.name);
                    if(pickUpManager == null)
                        Debug.LogError("PickUp_Manager is not available. Cannot add item.");
                }
            }
    }

    // Show prompt when player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PickUpPrompt != null && itemData != null)
        {
            PickUpPrompt.SetActive(true);
            Text[] textComponents = PickUpPrompt.GetComponentsInChildren<Text>();
            if(textComponents != null && textComponents.Length > 0)
            {
                textComponents[0].text = "Pick Up " + itemData.itemName;
            }
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PickUpPrompt != null)
        {
            PickUpPrompt.SetActive(false);
            Text[] textComponents = PickUpPrompt.GetComponentsInChildren<Text>();
            if(textComponents != null && textComponents.Length > 0)
            {
                textComponents[0].text = "";
            }
        }
    }
}
