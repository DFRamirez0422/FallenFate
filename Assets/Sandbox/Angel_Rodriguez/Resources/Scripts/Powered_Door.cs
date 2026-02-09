using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Powered_Door : CollidableObject
{
    [Header("UI Elements")]
    [SerializeField] private GameObject PoweredDoorPrompt;
    private GameObject _SpawnedPrompt;

    [Header("Power Settings")]
    [SerializeField] private bool isPowered = false; // Indicates if the door is powered
    private bool _doorOpened = false;
    [SerializeField] private Activate_Generators activateGenerators;
    [SerializeField] private Activate_Generators activate_Generator2;

     void Awake()
    {
        // Load the prompt prefab from the Resources folder
        PoweredDoorPrompt = Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription");
    }


    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
    }

    protected override void Update()
    {
        base.Update(); // Calls the Update method of CollidableObject

        // Check if both generators are activated to power the door
        if(activateGenerators.Activate_Generator && activate_Generator2.Activate_Generator)
        {
            isPowered = true;
        }
        else
        {
            isPowered = false;
        }

    }

    protected override void OnCollide(GameObject other)
    {
        // Check if the door is already opened
        if(_doorOpened) return;

        // Check for player input to open the door
        if (Input.GetButtonDown("Interact"))
        {
            if (isPowered)
            {
                OpenDoor();
                _doorOpened = true;
            }
        }
    }

    void OpenDoor()
    {
        transform.Translate(-1f, 0, 0); // Move the door up to simulate opening
        GetComponent<Collider2D>().enabled = false; // Disable the collider to allow passage
    }
    
    // Show the prompt when the player enters the trigger area
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _SpawnedPrompt = Instantiate(PoweredDoorPrompt);
            _SpawnedPrompt.SetActive(true);
            if(isPowered)
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
            _SpawnedPrompt.SetActive(false);
            _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }

}
