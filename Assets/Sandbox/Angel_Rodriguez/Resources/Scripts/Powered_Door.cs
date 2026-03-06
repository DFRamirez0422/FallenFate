using UnityEngine;
using UnityEngine.UI;

public class Powered_Door : CollidableObject
{
    [Header("Door Sprites")]
    [SerializeField] private Sprite Door_Closed;
    [SerializeField] private Sprite Door_Open;

    [Header("Door Lights Sprites")]
    [SerializeField] private SpriteRenderer Door_Light_Prefab;
    [SerializeField] private Sprite Door_Light_1_On;
    [SerializeField] private Sprite Door_Light_2_On;
    [SerializeField] private Sprite Door_Light_On;

    [Header("UI Elements")]
    [SerializeField] private GameObject PoweredDoorPrompt;
    private GameObject _SpawnedPrompt;

    [Header("Power Settings")]
    private bool _doorOpened = false;
    [SerializeField] private Activate_Generators activateGenerators;
    [SerializeField] private Activate_Generators activate_Generator2;
    private AudioSource _doorOpenSound;

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
        _doorOpenSound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
            if (activateGenerators.Activate_Generator && activate_Generator2.Activate_Generator)
            {
                Door_Light_Prefab.sprite = Door_Light_On;
            }
            else if (activateGenerators.Activate_Generator)
            {
                Door_Light_Prefab.sprite = Door_Light_1_On;
            }
            else if (activate_Generator2.Activate_Generator)
            {
                Door_Light_Prefab.sprite = Door_Light_2_On;
            }
    }

    protected override void OnCollide(GameObject other)
    {
        // Check if the door is already opened
        if(_doorOpened) return;

        // Check for player input to open the door
        if (Input.GetButtonDown("Interact"))
        {
            if (activateGenerators == null || activate_Generator2 == null)
            {
                Debug.LogError("Activate_Generators references are not set in the inspector.");
                return;
            }

            if (activateGenerators.Activate_Generator && activate_Generator2.Activate_Generator)
            {
                this.GetComponent<SpriteRenderer>().sprite = Door_Open; // Change sprite to open
                this.GetComponent<BoxCollider2D>().enabled = false; // Disable collider to allow passage
                _doorOpenSound.Play(); // Play door opening sound
                _doorOpened = true;
            }
        }
    }
    
    // Show the prompt when the player enters the trigger area
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _SpawnedPrompt = Instantiate(PoweredDoorPrompt);
            _SpawnedPrompt.SetActive(true);
            if (activateGenerators.Activate_Generator && activate_Generator2.Activate_Generator)
            {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Open Door";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "[x]";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
            }
            else
            {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "The door is not powered.";
            }
        }
    }

    // Hide the prompt when the player exits the trigger area
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PoweredDoorPrompt.SetActive(false);
            PoweredDoorPrompt.GetComponentsInChildren<Text>()[0].text = "";
            PoweredDoorPrompt.GetComponentsInChildren<Text>()[1].text = "";
            PoweredDoorPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }

}
