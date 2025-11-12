using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuickTimeEvent_JoseE : MonoBehaviour
{
    public Text promptText; // UI Text to display the key
    public Slider timerSlider; // UI Slider for the countdown
    public float qteDuration = 2f; // Duration of the QTE
    public KeyCode requiredKey; // The key the player needs to press

    // vvvvv Added by Jose E. from original file. vvvvv //

    /// <summary>
    /// Exposed public field that returns true if the quick time event was hit on time.
    /// </summary>
    public bool IsHitOnTime = false; // ADDED BY: Jose E.

    /// <summary>
    /// Exposed public field that returns true if the quick time event is still running.
    /// </summary>
    public bool IsStillRunning => qteActive; // ADDED BY: Jose E.

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
                IsHitOnTime = true; // ADDED BY: Jose E.
                yield break; // Exit coroutine on success
            }
            yield return null;
        }

        if (qteActive) // If QTE is still active after timer runs out
        {
            Debug.Log("QTE TO HELL WITH YOU!");
            promptText.text = "TO HELL WITH YOU!";
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