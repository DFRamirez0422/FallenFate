using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Needed if you're using UI Text

public class Counter : MonoBehaviour, IPointerClickHandler
{
    public Text countText;  // Drag the UI Text named "count" into this slot in the Inspector
    private int Count = 0;

    // This is called when the panel is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // right-click
        {
            Count++;

            // If under 4, show the "power up" message
            if (Count > 4)
            {
                countText.text = "Woah! You got a power up!!";
            }
            else
            {
                // Otherwise, just show the number
                countText.text = " " + Count;
            }
        }
    }
}
