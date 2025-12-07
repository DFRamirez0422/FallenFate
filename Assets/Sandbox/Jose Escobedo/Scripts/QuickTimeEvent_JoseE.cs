using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class QuickTimeEvent_JoseE : MonoBehaviour
{
    public Text promptText; // UI Text to display the key
    public Slider timerSlider; // UI Slider for the countdown
    public float qteDuration = 2f; // Duration of the QTE
    public KeyCode requiredKey; // The key the player needs to press

    // vvvvv Added by Jose E. from original file. vvvvv //

    [Tooltip("List of callbacks to be invoked upon hitting the QTE event on time.")]
    [SerializeField] private UnityEvent m_OnSuccessfulHit;

    [Tooltip("List of callbacks to be invoked upon not hitting the QTE on time.")]
    [SerializeField] private UnityEvent m_OnFailedHit;

    /// <summary>
    /// Exposed public field that returns true if the quick time event was hit on time.
    /// </summary>
    public bool IsHitOnTime = false; // ADDED BY: Jose E.

    /// <summary>
    /// Exposed public field that returns true if the quick time event is still running.
    /// </summary>
    public bool IsActive => qteActive; // ADDED BY: Jose E.

    // ^^^^^ Added by Jose E. from original file. ^^^^^ //

    private bool qteActive = false;

    public void StartQTE()
    {
        qteActive = true;
        promptText.text = requiredKey.ToString(); // Display the required key
        timerSlider.maxValue = qteDuration;
        timerSlider.value = qteDuration;
        StartCoroutine(QTECountdown());
    }

    IEnumerator QTECountdown()
    {
        float timer = qteDuration;
        while (timer > 0 && qteActive)
        {
            timer -= Time.deltaTime;
            timerSlider.value = timer;
            if (Input.GetKeyDown(requiredKey))
            {
                Debug.Log("QTE EPIC!");
                qteActive = false;
                promptText.text = "EPIC!";

                // ADDED BY: Jose E.
                Debug.Log("I hit on time!");
                IsHitOnTime = true; 
                m_OnSuccessfulHit?.Invoke();

                yield break; // Exit coroutine on success
            }
            yield return null;
        }

        if (qteActive) // If QTE is still active after timer runs out
        {
            Debug.Log("QTE TO HELL WITH YOU!");
            promptText.text = "TO HELL WITH YOU!";

            // ADDED BY: Jose E.
            Debug.Log("No, I fail the event...");
            m_OnFailedHit?.Invoke();

            qteActive = false;
        }
    }

    // Call this to stop the QTE if needed
    public void StopQTE()
    {
        qteActive = false;
        IsHitOnTime = false; // ADDED BY: Jose E.
        StopAllCoroutines(); // Stop any running QTE coroutines
    }
}