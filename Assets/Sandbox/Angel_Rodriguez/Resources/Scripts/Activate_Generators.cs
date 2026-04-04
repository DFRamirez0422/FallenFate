using UnityEngine;
using UnityEngine.UI;

public class Activate_Generators : CollidableObject
{
    [Header("Sprites")]
    [SerializeField] private Sprite Generator_Off;
    [SerializeField] private Sprite Generator_On;

    [Header("Generator Activation Settings")]
    public bool Activate_Generator = false;
    private bool _hasActivated = false;
    [SerializeField] private GameObject ActivateGeneratorPrompt;
    private GameObject _SpawnedPrompt;
    [SerializeField] private AudioSource GeneratorActivateSound;

    [SerializeField] private Item_Data item_Data;
    private PickUp_Manager _pickUp_Manager;

    // Initialize prompt references and use base Start method
    // Override the Start method to set up references
    protected override void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = Generator_Off; // Set initial sprite to off
        _pickUp_Manager = GameObject.FindGameObjectWithTag("PickUp_Manager").GetComponent<PickUp_Manager>();
        ActivateGeneratorPrompt = Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription");
        base.Start(); // Calls the Start method of CollidableObject

    }
    
    //Activate generator on collide and key press
    protected override void OnCollide(GameObject other)
    {
        if(_hasActivated) return;
        if (Input.GetButtonDown("Interact"))
        {
            Activate_Generator = true;
            _hasActivated = true;
            this.GetComponent<SpriteRenderer>().sprite = Generator_On; // Change sprite to on
            _pickUp_Manager.items.Add(item_Data); // Add item to inventory
            GeneratorActivateSound.Play();
        }
    }
    
    // Show prompt when collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
                _SpawnedPrompt = Instantiate(ActivateGeneratorPrompt);
                if(!Activate_Generator)
                {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Activate Generator";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "[x]";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
                }
                else if(Activate_Generator)
                {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Generator Activated";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
                }
        }

        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
                if(Activate_Generator)
                {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "Generator Activated";
                _SpawnedPrompt.SetActive(true);
                }
        }
    }

    // Hide prompt when player exits collision area
    private void OnCollisionExit2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            _SpawnedPrompt.SetActive(false);
            _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
            _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(_SpawnedPrompt);
        }
    }
    
}
