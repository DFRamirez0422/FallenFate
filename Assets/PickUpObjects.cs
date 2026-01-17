using UnityEngine;
using UnityEngine.UI;

public class PickUpObjects : CollidableObject
{
    [SerializeField] private Text promptText;
    [SerializeField] private Image promptBackground;

    

    protected override void Start()
    {
        base.Start();
        promptText = GetComponentInChildren<Text>();
        promptBackground = GetComponentInChildren<Image>();
        promptText.enabled = false;
        promptBackground.enabled = false;
    }
    protected override void OnCollide(GameObject other)
    {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Picked up " + gameObject.name);
                Destroy(gameObject);
            }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = true;
            promptBackground.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptText.enabled = false;
            promptBackground.enabled = false;
        }
    }
}
