using System.Linq.Expressions;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//I Have updated this script to use CollidableObject as a base class for consistency and better collision handling.

public class PickUpObjects : CollidableObject // Inherits from CollidableObject
{
    [SerializeField] private Text promptText;
    [SerializeField] private Image promptBackground;
    private PickUp_Manager pickUpManager;
    [SerializeField] private Item_Data itemData;
    [SerializeField] private string scriptableObjectPath;

    

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject and allows to be overridden by PickUpObjects Script
        promptText = GetComponentInChildren<Text>();
        promptBackground = GetComponentInChildren<Image>();
        promptText.enabled = false;
        promptBackground.enabled = false;
        if(scriptableObjectPath != null && scriptableObjectPath != "")
        {
            itemData = Resources.Load<Item_Data>(scriptableObjectPath);
        }
        else
        {
            Debug.LogError("Path to Item_Data is not set for " + gameObject.name);
        }

        pickUpManager = GameObject.Find("Item_PickUp_Manager").GetComponent<PickUp_Manager>();
        if(pickUpManager == null)
        {
            Debug.LogError("PickUp_Manager not found in the scene.");
        }

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
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Picked up " + gameObject.name);
                if (gameObject != null && itemData != null)
                {
                        itemData.collected = true;
                        pickUpManager.items.Add(itemData);
                        Debug.Log("Added " + itemData.itemName + " to PickUp_Manager");
                        var copy = this.gameObject;
                        Destroy(copy);
                }
                else
                {
                    Debug.LogError("Item_Data is not assigned for " + gameObject.name);
                }
            }
    }

    // Show prompt when player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = true;
            promptBackground.enabled = true;
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = false;
            promptBackground.enabled = false;
        }
    }
}
