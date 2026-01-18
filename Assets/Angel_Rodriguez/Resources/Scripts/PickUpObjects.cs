using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PickUpObjects : CollidableObject // Inherits from CollidableObject
{
    [SerializeField] private Text promptText;
    [SerializeField] private Image promptBackground;
    private PickUp_Manager pickUpManager;

    

    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject and allows to be overridden by PickUpObjects Script
        promptText = GetComponentInChildren<Text>();
        promptBackground = GetComponentInChildren<Image>();
        promptText.enabled = false;
        promptBackground.enabled = false;
    }

    // Override the OnCollide method to implement pick-up logic
    protected override void OnCollide(GameObject other)
    {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Picked up " + gameObject.name);

                if(this.gameObject.name == "Memento1")
                {
                    // Gets PickUp_Manager and activate Memento1
                    pickUpManager = GameObject.Find("Item_PickUp_Manager").GetComponent<PickUp_Manager>();
                    pickUpManager.ActivateMemento1();
                }
                else if(this.gameObject.name == "Memento2")
                {
                    // Gets PickUp_Manager and activate Memento2
                    pickUpManager = GameObject.Find("Item_PickUp_Manager").GetComponent<PickUp_Manager>();
                    pickUpManager.ActivateMemento2();
                }
                else if(this.gameObject.name == "Memento3")
                {
                    // Gets PickUp_Manager and activate Memento3
                    pickUpManager = GameObject.Find("Item_PickUp_Manager").GetComponent<PickUp_Manager>();
                    pickUpManager.ActivateMemento3();
                }
                else{
                    Debug.Log("No matching memento found.");
                }

                Destroy(gameObject);
            }
    }

    // Show prompt when player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = true;
            promptBackground.enabled = true;
        }
    }

    // Hide prompt when player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = false;
            promptBackground.enabled = false;
        }
    }
}
