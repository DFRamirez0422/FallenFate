using UnityEngine;
using UnityEngine.UI;

public class OpenDoors : CollidableObject
{
    [Header("Door Settings")]
    private PickUp_Manager pickUpManager;
    [SerializeField] private Item_Data Key; // Key required to open the door
    private AudioSource _doorOpenSound; // Sound to play when the door opens

    private bool _isDoorOpen = false; // Flag to check if the door is already open

    [Header("Animation Settings")]

    [SerializeField] private Animator DoorAnimator;

    [Header("UI Elements")]
    [SerializeField] private GameObject OpenDoorPrompt;
    private GameObject _SpawnedPrompt;
    
        void Awake()
    {
       DoorAnimator = GetComponent<Animator>();
         _doorOpenSound = GetComponent<AudioSource>();
    }
   
   // Initialize references and use base Start method and check for nulls
   // Override the Start method to set up references
   protected override void Start()
    {
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
        if (_isDoorOpen) return; // If the door is already open, do nothing
        if (Input.GetButtonDown("Interact"))
        {  
                // Check if the player has the required key in the PickUp_Manager
                if (pickUpManager.items.Contains(Key))
                {
                    OpenDoor();
                    _isDoorOpen = true;
                }
        }
    }
    
    // Method to open the door
    private void OpenDoor()
    {
        Debug.Log("Door Opened");
        _doorOpenSound.Play();
        DoorAnimator.SetBool("HasKey", true);
    }

    // Show prompt when collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pickUpManager.items.Contains(Key)){
            _SpawnedPrompt = Instantiate(OpenDoorPrompt);
            _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Open Door";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "[x]";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
            _SpawnedPrompt.SetActive(true);
           }
            else
            {
                _SpawnedPrompt = Instantiate(OpenDoorPrompt);
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "locked. Find the key.";
                _SpawnedPrompt.SetActive(true);
            }
        }
    }

    // Hide prompt when player exits collision area
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OpenDoorPrompt.SetActive(false);
            OpenDoorPrompt.GetComponentsInChildren<Text>()[0].text = "";
            OpenDoorPrompt.GetComponentsInChildren<Text>()[1].text = "";
            OpenDoorPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }
}
