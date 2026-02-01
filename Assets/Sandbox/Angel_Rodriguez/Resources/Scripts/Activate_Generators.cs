using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Activate_Generators : CollidableObject
{
    public bool Activate_Generator = false;
    [SerializeField] private GameObject ActivateGeneratorPrompt;

    // Initialize prompt references and use base Start method
    // Override the Start method to set up references
    protected override void Start()
    {
        base.Start(); // Calls the Start method of CollidableObject

    }
    
    //Activate generator on collide and key press
    protected override void OnCollide(GameObject other)
    {
        if (Input.GetButtonDown("Interact"))
        {
            Activate_Generator = true;
            Debug.Log(this.gameObject.name + " Activated");
        }
    }
    
    // Show prompt when collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            ActivateGeneratorPrompt.SetActive(true);
            ActivateGeneratorPrompt.GetComponentsInChildren<Text>()[0].text = "Activate Generator";
        }
    }

    // Hide prompt when player exits collision area
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ActivateGeneratorPrompt.SetActive(false);
            ActivateGeneratorPrompt.GetComponentsInChildren<Text>()[0].text = "";
        }
    }
    
}
