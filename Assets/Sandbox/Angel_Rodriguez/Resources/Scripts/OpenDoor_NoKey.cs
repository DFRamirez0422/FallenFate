using UnityEngine;
using UnityEngine.UI;

public class OpenDoor_NoKey : CollidableObject
{
    private AudioSource _doorOpenSound; // Sound to play when the door opens
    private Animator DoorAnimator; // Animator for the door
    [Header("UI Elements")]
    [SerializeField] private GameObject OpenDoorPrompt;
    private GameObject _SpawnedPrompt;
    private bool _doorOpened = false;

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
        _doorOpenSound = GetComponent<AudioSource>();
        DoorAnimator = GetComponent<Animator>();
    }

    protected override void OnCollide(GameObject other)
    {
        // Check if the door is already opened to prevent multiple openings
        if(_doorOpened) return;

        // Check for player input to open the door
        if (Input.GetButtonDown("Interact"))
        {  
            OpenDoor(); 
        }
    }
    
    // Method to open the door
    private void OpenDoor()
    {
        Debug.Log("Door Opened");
        _doorOpenSound.Play();
        DoorAnimator.SetBool("Open", true);
        _doorOpened = true;
    }
    
    // Show the prompt when the player enters the trigger area
    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs") || hitCollider.CompareTag("Player"))
        {
                _SpawnedPrompt = Instantiate(OpenDoorPrompt);
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Open Door";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "[x]";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
        }   
    }

    // Hide the prompt when the player exits the trigger area
    void OnCollisionExit2D(Collision2D collision)
    {

        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs") || hitCollider.CompareTag("Player"))
        {
            _SpawnedPrompt.SetActive(false);
            _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }

}
