using UnityEngine;

public class OpenDoor_NoKey : CollidableObject
{
    [SerializeField] private Vector3 _TranslatePosition;

    [Header("UI Elements")]
    [SerializeField] private GameObject OpenDoorPrompt;
    private GameObject _SpawnedPrompt;
    private bool _doorOpened = false;

     void Awake()
    {
        // Load the prompt prefab from the Resources folder
        OpenDoorPrompt = Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription"); 
    }

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
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
        transform.Translate(_TranslatePosition); // Move the door up to simulate opening
        GetComponent<Collider2D>().enabled = false; // Disable the collider to allow passage
        _doorOpened = true;
    }
    
    // Show the prompt when the player enters the trigger area
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
                _SpawnedPrompt = Instantiate(OpenDoorPrompt);
                _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[0].text = "Open Door";
                _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[1].text = "[x]";
                _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
        }   
    }

    // Hide the prompt when the player exits the trigger area
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _SpawnedPrompt.SetActive(false);
            _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[0].text = "";
            _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[1].text = "";
            _SpawnedPrompt.GetComponentsInChildren<UnityEngine.UI.Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }

}
