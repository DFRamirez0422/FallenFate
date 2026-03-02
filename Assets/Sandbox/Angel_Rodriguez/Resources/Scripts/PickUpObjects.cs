using UnityEngine;
using UnityEngine.UI;


//I Have updated this script to use CollidableObject as a base class for consistency and better collision handling.

public class PickUpObjects : CollidableObject // Inherits from CollidableObject
{

    [Header("Pick-Up Settings")]
    private PickUp_Manager pickUpManager;
    public Item_Data itemData;
    public bool isPickedUp = false;

    [Header("UI Elements")]
    [Tooltip("UI Prompt to show when player can pick up the item")]
    [SerializeField] private GameObject PickUpPrompt;
    private GameObject PickUpPromptPrefab;

    [Header("Audio Settings")]
    [Tooltip("Sound to play when item is picked up")]
    [SerializeField] private AudioSource PickUpSound;


    

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject and allows to be overridden by PickUpObjects Script
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
    }

    // Override the OnCollide method to implement pick-up logic
    protected override void OnCollide(GameObject other)
    {
        if(isPickedUp) return; // If the item is already picked up, do nothing

        //THis for the items that are used for the EnemySpawner
            if(Input.GetButtonDown("Interact"))
            {

                PickUpSound.Play();
                pickUpManager.items.Add(itemData);
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the item visually
                isPickedUp = true; // Set the flag to true when the item is picked up
                Destroy(this.gameObject, PickUpSound.clip.length);
            }
    }

    // Show prompt when player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpPromptPrefab = Instantiate(PickUpPrompt);
            PickUpPromptPrefab.SetActive(true);
            PickUpPromptPrefab.GetComponentsInChildren<Text>()[0].text = "Pick up " + itemData.itemName;
            PickUpPromptPrefab.GetComponentsInChildren<Text>()[1].text = "[x]";
            PickUpPromptPrefab.GetComponentsInChildren<Text>()[2].text = "";
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpPrompt.SetActive(false);
            PickUpPrompt.GetComponentsInChildren<Text>()[0].text = "";
            PickUpPrompt.GetComponentsInChildren<Text>()[1].text = "";
            PickUpPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(PickUpPromptPrefab);

        }
    }
}
