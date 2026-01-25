using UnityEngine;
using UnityEngine.UI;

public class OpenDoors : CollidableObject
{
    [Header("Door Settings")]
    private PickUp_Manager pickUpManager;
    [SerializeField] private Item_Data Key; // Key required to open the door
    [SerializeField] private Text promptText;
    [SerializeField] private Image promptBackground;

    [Header("Generator Option")]
    [SerializeField] private bool Usegenerator; // If true, door requires generators to open else requires key
    [SerializeField] private Activate_Generators Generator1;
    [SerializeField] private Activate_Generators Generator2;
   
   // Initialize references and use base Start method and check for nulls
   // Override the Start method to set up references
   protected override void Start()
    {
        promptText = GetComponentInChildren<Text>();
        promptBackground = GetComponentInChildren<Image>();
        promptText.enabled = false;
        promptBackground.enabled = false;
        pickUpManager = GameObject.Find("Item_PickUp_Manager").GetComponent<PickUp_Manager>();
        
        // Check for null references
        if(pickUpManager == null)
        {
            Debug.LogError("PickUp_Manager not found in the scene.");
        }
        
        if(Key == null)
        {
            Debug.LogError("Missing Key");
        }
        
        base.Start(); // Calls the Start method of CollidableObject
    }

    // Override the OnCollide method to implement door opening logic
    protected override void OnCollide(GameObject other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {  
            if (Usegenerator) // Check if door uses generators to open
            {
                // Check if both generators are activated
                if(Generator1.Activate_Generator && Generator2.Activate_Generator)
                {
                    Debug.Log("Door Unlocked with Generators");
                    OpenDoor();
                }
                else
                {
                    Debug.Log("You need to activate both generators to open this door.");
                }
            }
            else if (!Usegenerator) // Door uses key to open
            {
                // Check if the player has the required key in the PickUp_Manager
                if (pickUpManager.items.Contains(Key) && Key.collected)
                {
                    Debug.Log("Door Unlocked with " + Key.itemName);
                    OpenDoor();
                }
                else
                {
                    Debug.Log("You need " + Key.itemName + " to open this door.");
                }
            }
        }
    }
    
    // Method to open the door
    private void OpenDoor()
    {
        Debug.Log("Door Opened");
        Destroy(this.gameObject);
    }

    // Show prompt when collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            promptText.enabled = true;
            promptBackground.enabled = true;
        }
    }

    // Hide prompt when player exits collision area
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            promptText.enabled = false;
            promptBackground.enabled = false;
        }
    }
}
