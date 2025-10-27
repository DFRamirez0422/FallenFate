// This code assigns the lanes where the notes will fall in
// When the right key is pressed it'll find the closest note and check if it's a hit
// Handles the press animations

using System.Collections;
using System.Collections.Generic; 
using UnityEngine; 

public class QTELane : MonoBehaviour
{
    public KeyCode assignedKey = KeyCode.W; // key that triggers this lane

    RectTransform rt; // stores the lane's position and size
    public RectTransform Rect => rt; // allows other scripts to read rect

    [HideInInspector] public List<QTENote> activeNotes = new List<QTENote>(); // keeps track of all notes in this lane

    [Header("Press Effect")]
    public float shrinkScale = 0.85f; // how small the lane gets when pressed
    public float shrinkSpeed = 10f; // how fast it shrinks
    public float returnSpeed = 10f; // how fast it goes back
    public bool useUnscaledTime = true; // ignore time scale for smooth UI animation

    Vector3 baseScale; // stores the original scale
    bool isHolding; // checks if the key is being held down

    void Awake()
    {
        rt = GetComponent<RectTransform>(); // gets the RectTransform of the lane
        baseScale = rt.localScale; // saves the lane's normal size
    }

    void Update()
    {
        // if key pressed down
        if (Input.GetKeyDown(assignedKey))
        {
            isHolding = true; // mark as being held
            JudgeClosestNote(); // check if a note should be hit
        }

        // if key released
        if (Input.GetKeyUp(assignedKey))
        {
            isHolding = false; // stop holding
        }

        // update animation for pressing or releasing
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime; // use correct time
        Vector3 targetScale = isHolding ? baseScale * shrinkScale : baseScale; // decide target size
        rt.localScale = Vector3.Lerp(rt.localScale, targetScale, dt * (isHolding ? shrinkSpeed : returnSpeed)); // smoothly change scale
    }

    void JudgeClosestNote()
    {
        if (activeNotes.Count == 0) return; // stop if no notes exist

        QTENote closest = null; // store closest note
        float closestDist = float.MaxValue; // set max distance
        float hitY = QTEManager.Instance.hitBox.rectTransform.anchoredPosition.y; // get hitbox Y position

        // find note closest to the hitbox
        foreach (var note in activeNotes)
        {
            if (note == null) continue; // skip empty ones
            float dist = Mathf.Abs(note.Rect.anchoredPosition.y - hitY); // find distance to hitbox
            if (dist < closestDist) // check if closer
            {
                closestDist = dist; // update shortest distance
                closest = note; // set closest note
            }
        }

        // tell the closest note to check if it was hit
        if (closest != null)
            closest.TryHitByLane(); // run hit check
    }
}