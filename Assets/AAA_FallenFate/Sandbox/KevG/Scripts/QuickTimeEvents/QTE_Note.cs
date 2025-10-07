// Quick Time Event (QTE)
// Expanding pulse circle and smooth pop effect
// Code by Luis Espinoza

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QTE_Note : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform mainCircle;        // the fixed circle in the middle
    public Image pulseCircle;               // the expanding circle around it
    public TextMeshProUGUI arrowText;       // the arrow symbol inside the circle
    public TextMeshProUGUI instructionText; // extra text below the symbol (for instructions)

    [Header("QTE Settings")]
    public float lifetime = 1.5f;           // how long until the pulse reaches the main circle
    public float popScaleMultiplier = 1.3f; // how much bigger it grows when pressed
    public float popDuration = 0.2f;        // how long the pop effect lasts 

    private float timer;                    // counts down until the QTE ends
    private KeyCode neededKey;              // the arrow key the player must press
    private bool active = false;            // tracks if the note is currently running
    private Vector3 originalScale;          // stores the original scale of the note

    void Start()
    {
        timer = lifetime; // set timer to lifetime at the start

        // make pulse start small
        float baseSize = mainCircle.sizeDelta.x;
        pulseCircle.rectTransform.sizeDelta = new Vector2(baseSize * 0.3f, baseSize * 0.3f);

        // randomize arrow key and show correct symbol
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0: neededKey = KeyCode.UpArrow;    arrowText.text = "↑"; break;
            case 1: neededKey = KeyCode.RightArrow; arrowText.text = "→"; break;
            case 2: neededKey = KeyCode.DownArrow;  arrowText.text = "↓"; break;
            case 3: neededKey = KeyCode.LeftArrow;  arrowText.text = "←"; break;
        }

        // NEW: show instruction text under the arrow (optional)
        if (instructionText != null)
            instructionText.text = "Press the matching arrow key!";

        originalScale = transform.localScale; // save original size of the note
        active = true; // set as active so it runs
    }

    void Update()
    {
        if (!active) return; // don’t run if not active

        timer -= Time.deltaTime; // decrease timer each frame
        float progress = 1f - (timer / lifetime); // calculate how far along we are

        // grow pulse from small to match main circle
        float baseSize = mainCircle.sizeDelta.x;
        float currentSize = Mathf.Lerp(baseSize * 0.3f, baseSize, progress);
        pulseCircle.rectTransform.sizeDelta = new Vector2(currentSize, currentSize);

        // checks if player hits the right key
        if (Input.GetKeyDown(neededKey))
        {
            Debug.Log("HIT");
            active = false;
            StopAllCoroutines(); // stops any running coroutines
            StartCoroutine(SmoothPopAndDestroy()); // start smooth pop effect
        }
        // checks if player pressed the wrong key
        else if (Input.GetKeyDown(KeyCode.UpArrow) ||
                 Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("WRONG KEY");

        }

        // checks if player misses (time runs out)
        if (progress >= 1f)
        {
            Debug.Log("MISSED");
            active = false;
            Destroy(gameObject); 
        }
    }

    private IEnumerator SmoothPopAndDestroy() // smooth grow then shrink then destroy
    {
        Vector3 targetScale = originalScale * popScaleMultiplier; // target bigger size

        // first half: scale up
        float halfDuration = popDuration * 0.5f;
        float t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, lerp); // smooth increase
            yield return null;
        }

        // second half: scale back down
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, lerp); // smooth return
            yield return null;
        }

        Destroy(gameObject); // remove the note after animation
    }
}
