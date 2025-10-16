using NPA_PlayerPrefab.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string sentence;
    public Sprite characterSprite;
}

[System.Serializable]
public class Dialogue
{
    public DialogueLine[] lines;
}

public class DialogueController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public Image characterImage;
    public GameObject dialogueBox;

    [Header("References")]
    public PlayerController playerController;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private DialogueLine currentLine;

    private bool isTyping = false;
    private bool canAdvance = false;
    private float inputDelay = 0.2f;
    private float nextInputTime = 0f;

    private bool dialogueActive = false;

    public delegate void DialogueEndedHandler();
    public event DialogueEndedHandler OnDialogueEnded;

    void Start()
    {
        dialogueBox.SetActive(false);

        if (playerController == null)
            Debug.LogWarning("DialogueController: PlayerController is not assigned.");
        if (dialogueText == null)
            Debug.LogError("DialogueController: Missing dialogueText reference!");
        if (speakerNameText == null)
            Debug.LogError("DialogueController: Missing speakerNameText reference!");
    }

    void Update()
    {
        if (!dialogueBox.activeSelf || Time.time < nextInputTime)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentLine.sentence;
                isTyping = false;
                canAdvance = true;
            }
            else if (canAdvance)
            {
                DisplayNextLine();
            }

            nextInputTime = Time.time + inputDelay;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: Tried to start an empty dialogue.");
            return;
        }

        dialogueBox.SetActive(true);
        LockPlayerControls(true);
        dialogueActive = true;

        dialogueQueue.Clear();
        foreach (DialogueLine line in dialogue.lines)
            dialogueQueue.Enqueue(line);

        DisplayNextLine();
        nextInputTime = Time.time + inputDelay;
    }

    private void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = dialogueQueue.Dequeue();

        if (speakerNameText != null)
            speakerNameText.text = string.IsNullOrEmpty(currentLine.speakerName) ? "???" : currentLine.speakerName;

        if (characterImage != null)
            characterImage.sprite = currentLine.characterSprite != null ? currentLine.characterSprite : null;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        canAdvance = false;

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        canAdvance = true;
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        LockPlayerControls(false);
        dialogueQueue.Clear();
        dialogueActive = false;

        OnDialogueEnded?.Invoke();
    }

    private void LockPlayerControls(bool state)
    {
        if (playerController != null)
            playerController.enabled = !state;

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}