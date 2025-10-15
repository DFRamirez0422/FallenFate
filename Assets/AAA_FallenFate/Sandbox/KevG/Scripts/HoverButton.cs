using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
   
    private Vector3 originalScale; // store original object size

    public float hoverScaleMultiplier = 1.2f; // scaling multiplier when hovered
    public float clickScaleMultiplier = 1.4f; // scaling multiplier when clicked
    public float scaleSpeed = 10f;            // how fast the scaling transition happens

    private Vector3 objectScale; // the size of the object is trying to reach

    private bool isHovered = false; // tracks whether the pointer is currently over the object

    void Start()
    {
        originalScale = transform.localScale; // starting size
        objectScale = originalScale; // sets object starting size
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, objectScale, Time.unscaledDeltaTime * scaleSpeed); // smoothly transitions toward the desired object size
    }

    public void OnPointerEnter(PointerEventData eventData) // triggers when pointer is on the object
    {
        isHovered = true;
        objectScale = originalScale * hoverScaleMultiplier; // increases original scale with multiplier
    }

    public void OnPointerExit(PointerEventData eventData) // triggers when pointer leaves the object
    {
        isHovered = false;
        objectScale = originalScale; // returns back to original size
    }

    public void OnPointerClick(PointerEventData eventData) // triggers when pointer clicks on object
    {
        StopAllCoroutines(); // stops any coroutines currently running on this script

        StartCoroutine(ClickEffect()); // starts the coroutine that handles the click pop animation

    }

    private System.Collections.IEnumerator ClickEffect() // what happens when clicked
    {
        objectScale = originalScale * clickScaleMultiplier; // increases original size with multiplier
        yield return new WaitForSecondsRealtime(0.1f); // adds a delay for a "smoother pop effect"
        objectScale = isHovered ? originalScale * hoverScaleMultiplier : originalScale; // if still hovered, go back to hover size; otherwise return to original size
    }
}

