using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    }

    // Override the OnCollide method to implement pick-up logic
    protected override void OnCollide(GameObject other)
    {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Picked up " + gameObject.name);

                Destroy(gameObject);
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
