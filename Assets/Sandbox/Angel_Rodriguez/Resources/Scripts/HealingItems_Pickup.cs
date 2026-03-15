using UnityEngine;
using UnityEngine.UI;

public class HealingItems_Pickup : CollidableObject
{
    bool _hasActivated;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] private GameObject _UIPrompt;
    GameObject UI_Action;
    [SerializeField] private Item_Data item_Data;
    [SerializeField] private AudioSource PickUp_Sound;
    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    
    //Activate generator on collide and key press
    protected override void OnCollide(GameObject other)
    {
        if(_hasActivated) return;
        if (Input.GetButtonDown("Interact"))
        {
            if(playerHealth.m_CurrentHealth < playerHealth.MaxHealth)
            {
                playerHealth.ChangeHealth(1);
                PickUp_Sound.Play();
                Destroy(this.gameObject, PickUp_Sound.clip.length);
                
            }
            else{}
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Action = Instantiate(_UIPrompt);
            UI_Action.SetActive(true);
            UI_Action.GetComponentsInChildren<Text>()[0].text = "Pick up " + item_Data.itemName;
            UI_Action.GetComponentsInChildren<Text>()[1].text = "[x]";
            UI_Action.GetComponentsInChildren<Text>()[2].text = "";
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
