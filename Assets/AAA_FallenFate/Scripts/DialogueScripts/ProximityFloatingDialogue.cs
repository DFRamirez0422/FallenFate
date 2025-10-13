using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueLineTrigger
{
    [TextArea(2, 5)] public string line;             // The text to display
    [Range(0f, 1f)] public float chanceToSpeak = 1f; // Chance to display
    public bool repeatOnReentry = true;              // Whether line repeats

    [Tooltip("If true, uses this object's collider as the trigger. Otherwise, assign an external trigger.")]
    public bool useAssignedTrigger = true;          // Use this GameObject's collider instead of external one

    [Tooltip("Optional: Assign an external trigger collider if 'Use Assigned Collider' is disabled.")]
    public Collider externalTrigger;                 // Optional external trigger
}

public class ProximityFloatingDialogue : MonoBehaviour
{
    [Header("Dialogue Lines")]
    public DialogueLineTrigger[] dialogueLines;

    [Header("UI Settings")]
    public GameObject floatingTextPrefab;
    public Vector3 textOffset = new Vector3(0f, 1f, 0f);
    public float displayTime = 3f;

    private GameObject currentTextInstance;
    private bool[] hasTriggered;

    void Start()
    {
        // Initialize tracking for each line
        hasTriggered = new bool[dialogueLines.Length];

        // Add listeners to all external triggers
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            var dlg = dialogueLines[i];
            if (!dlg.useAssignedTrigger && dlg.externalTrigger != null)
            {
                TriggerListener listener = dlg.externalTrigger.gameObject.GetComponent<TriggerListener>();
                if (listener == null)
                    listener = dlg.externalTrigger.gameObject.AddComponent<TriggerListener>();

                int index = i; // capture index for closure
                listener.OnPlayerEnter = (col) =>
                {
                    if (col.CompareTag("Player"))
                        TryShowLine(index);
                };
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Check all lines using this GameObject's collider
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            if (dialogueLines[i].useAssignedTrigger)
                TryShowLine(i);
        }
    }

    private void TryShowLine(int index)
    {
        var dlg = dialogueLines[index];

        if (!dlg.repeatOnReentry && hasTriggered[index])
            return;

        if (Random.value > dlg.chanceToSpeak)
            return;

        ShowFloatingText(dlg.line);
        hasTriggered[index] = true;
    }

    private void ShowFloatingText(string text)
    {
        if (floatingTextPrefab == null) return;

        // Destroy previous text to prevent overlap
        if (currentTextInstance != null)
            Destroy(currentTextInstance);

        currentTextInstance = Instantiate(
            floatingTextPrefab,
            transform.position + textOffset,
            Quaternion.identity,
            transform
        );

        TextMeshPro tmp = currentTextInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
            tmp.text = text;

        if (currentTextInstance.GetComponent<Billboard>() == null)
            currentTextInstance.AddComponent<Billboard>();

        Destroy(currentTextInstance, displayTime);
    }
}

// Helper class to relay trigger events from external trigger colliders
public class TriggerListener : MonoBehaviour
{
    public System.Action<Collider> OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerEnter?.Invoke(other);
    }
}