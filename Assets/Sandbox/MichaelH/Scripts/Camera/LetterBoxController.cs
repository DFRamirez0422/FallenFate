using UnityEngine;

public class LetterBoxController : MonoBehaviour
{
    public RectTransform topBar;
    public RectTransform bottomBar;
    
    public float targetHeight = 150f;
    public float speed = 5f;
    
    float currentHeight;

    public void EnableBars()
    {
        gameObject.SetActive(true);
        currentHeight = targetHeight;
    }

    public void DisableBars()
    {
        currentHeight = 0f; // Reset height
        
        // Disable GO after animation
        CancelInvoke(nameof(DisableObject));
        Invoke(nameof(DisableObject), speed);
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Smoothly interpolate the height of the bars towards the target height
        float top = Mathf.Lerp(topBar.sizeDelta.y, currentHeight, Time.deltaTime * speed);
        float bottom =  Mathf.Lerp(bottomBar.sizeDelta.y, currentHeight, Time.deltaTime * speed);
        
        topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, top);
        bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, bottom);
    }
}
