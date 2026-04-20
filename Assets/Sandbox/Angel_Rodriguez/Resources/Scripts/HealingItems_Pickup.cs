using UnityEngine;
using UnityEngine.UI;

public class HealingItems_Pickup : CollidableObject
{
    bool _hasActivated; // to check if healed player
    
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] private GameObject _UIPrompt;
    private GameObject UI_Action;
    [SerializeField] private Item_Data item_Data;
    [SerializeField] private AudioSource PickUp_Sound;
    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    
    // overrides the OnCollide function
    protected override void OnCollide(GameObject other)
    {
        if(_hasActivated) return;
        if (Input.GetButtonDown("Interact"))
        {
            
            // Heals player if not at max health
            if(playerHealth.m_CurrentHealth < playerHealth.MaxHealth)
            {
                UI_Action.SetActive(false);
                Destroy(UI_Action);
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false; // Disable the collider to prevent multiple triggers
                this.gameObject.GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent multiple triggers
                playerHealth.ChangeHealth(1);
                PickUp_Sound.Play();
                Destroy(this.gameObject, PickUp_Sound.clip.length);
            }
            else
            {
            }
        }
    }
    
    // Shows propmt when player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hitboxs") || other.CompareTag("Player"))
        {
            PlayerHealth _playerHealth = other.GetComponentInParent<PlayerHealth>();
            if(_playerHealth.m_CurrentHealth < _playerHealth.MaxHealth){
                UI_Action = Instantiate(_UIPrompt);
                UI_Action.GetComponentsInChildren<Text>()[0].text = "Pick up " + item_Data.itemName;
                UI_Action.GetComponentsInChildren<Text>()[1].text = "[x]";
                UI_Action.GetComponentsInChildren<Text>()[2].text = "";
                UI_Action.SetActive(true);
            }
             else
            {
                UI_Action = Instantiate(_UIPrompt);
                UI_Action.GetComponentsInChildren<Text>()[0].text = "";
                UI_Action.GetComponentsInChildren<Text>()[1].text = "";
                UI_Action.GetComponentsInChildren<Text>()[2].text = "Health is full";
                UI_Action.SetActive(true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Hitboxs") || collision.CompareTag("Player")){
           PlayerHealth _playerHealth = collision.GetComponentInParent<PlayerHealth>();
         if(_playerHealth.m_CurrentHealth < _playerHealth.MaxHealth)
            {
               UI_Action.GetComponentsInChildren<Text>()[0].text = "Pick up " + item_Data.itemName;
               UI_Action.GetComponentsInChildren<Text>()[1].text = "[x]";
               UI_Action.GetComponentsInChildren<Text>()[2].text = "";
            }
            else
            {
                UI_Action.GetComponentsInChildren<Text>()[0].text = "";
                UI_Action.GetComponentsInChildren<Text>()[1].text = "";
                UI_Action.GetComponentsInChildren<Text>()[2].text = "Health is full";
            }
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hitboxs") || other.CompareTag("Player"))
        {
            UI_Action.SetActive(false);
            UI_Action.GetComponentsInChildren<Text>()[0].text = "";
            UI_Action.GetComponentsInChildren<Text>()[1].text = "";
            UI_Action.GetComponentsInChildren<Text>()[2].text = "";
            Destroy(UI_Action);
        }
    }

   /*
    public static class ActionDescriptionHelper
   {
    public static void SetPromptText(GameObject prompt, string description, string buttonHint, string status)
    {
        if (prompt == null) return;
        var texts = prompt.GetComponentsInChildren<Text>();
        var desc = System.Array.Find(texts, t => t.gameObject.name == "Description");
        var btn = System.Array.Find(texts, t => t.gameObject.name == "Button_press_prompt");
        var statusText = System.Array.Find(texts, t => t.gameObject.name == "Unlock_NotMeet");
        if (desc != null) desc.text = description ?? "";
        if (btn != null) btn.text = buttonHint ?? "";
        if (statusText != null) statusText.text = status ?? "";
    }
    }
    */
}
