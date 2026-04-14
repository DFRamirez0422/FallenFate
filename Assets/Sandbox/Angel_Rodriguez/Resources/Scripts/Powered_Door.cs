using UnityEngine;
using UnityEngine.UI;

public class Powered_Door : CollidableObject
{
    [Header("Door Sprites")]
    [SerializeField] private Sprite Door_Closed;
    [SerializeField] private Sprite Door_Open;

    [Header("UI Elements")]
    [SerializeField] private GameObject PoweredDoorPrompt;
    private GameObject _SpawnedPrompt;

    [Header("Power Settings")]
    private bool _doorOpened = false;
    public Activate_Generators activateGenerators;
    public Activate_Generators activate_Generator2;
    private AudioSource _doorOpenSound;

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
        _doorOpenSound = GetComponent<AudioSource>();
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
                if (_SpawnedPrompt != null)
                {
                    Destroy(_SpawnedPrompt);
                    _SpawnedPrompt = null;
                }
            }
        }
    }

    private void ApplyPoweredDoorPrompt()
    {
        if (_SpawnedPrompt == null || _doorOpened) return;
        if (activateGenerators == null || activate_Generator2 == null)
        {
            Debug.LogError("Activate_Generators references are not set in the inspector.");
            return;
        }

        Text[] texts = _SpawnedPrompt.GetComponentsInChildren<Text>();
        bool g1 = activateGenerators.Activate_Generator;
        bool g2 = activate_Generator2.Activate_Generator;

        if (g1 && g2)
        {
            // [2] is Unlock_NotMeet (red); filling it overlaps the main line. Keep status on Description [0].
            texts[0].text = "Open Door\nBoth Generators Activated";
            texts[1].text = "[x]";
            texts[2].text = "";
        }
        else
        {
            texts[0].text = "";
            texts[1].text = "";
            if (!g1 && !g2)
                texts[2].text = "Electric Exit Door (0 Generators Activated)";
            else
                texts[2].text = "Door Power 50%";
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            _SpawnedPrompt = Instantiate(PoweredDoorPrompt);
            _SpawnedPrompt.SetActive(true);
            ApplyPoweredDoorPrompt();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            ApplyPoweredDoorPrompt();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            if (_SpawnedPrompt != null)
            {
                Destroy(_SpawnedPrompt);
                _SpawnedPrompt = null;
            }
        }
    }

}
