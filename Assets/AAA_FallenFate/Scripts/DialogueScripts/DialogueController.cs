using NPA_PlayerPrefab.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string[] sentences;
    public Sprite[] characterSprites;
}

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public GameObject dialogueBox;

    [Header("References")]
    public PlayerController playerController;  // assign in Inspector

    private Queue<string> sentences;
    private Queue<Sprite> characterSprites;

    private bool canAdvance = false;  // now used properly
    private bool isTyping = false;    // track if sentence is being typed
    private float inputDelay = 0.2f;  // small buffer to prevent skipping
    private float nextInputTime = 0f;

    void Start()
    {
        sentences = new Queue<string>();
        characterSprites = new Queue<Sprite>();
        dialogueBox.SetActive(false);

        if (playerController == null)
        {
            Debug.LogError("DialogueController: PlayerController is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        if (dialogueBox.activeSelf && Time.time >= nextInputTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // Instantly finish current sentence
                    StopAllCoroutines();
                    dialogueText.text = sentences.Peek(); // show full current sentence
                    isTyping = false;
                    canAdvance = true;
                }
                else if (canAdvance)
                {
                    DisplayNextSentence();
                }

                nextInputTime = Time.time + inputDelay;
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        LockPlayerControls(true);

        sentences.Clear();
        characterSprites.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (Sprite sprite in dialogue.characterSprites)
        {
            characterSprites.Enqueue(sprite);
        }

        // show first sentence
        DisplayNextSentence();
        nextInputTime = Time.time + inputDelay;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Sprite sprite = characterSprites.Count > 0 ? characterSprites.Dequeue() : null;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        if (sprite != null) characterImage.sprite = sprite;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        canAdvance = false;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }

        isTyping = false;
        canAdvance = true;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        LockPlayerControls(false);
    }

    void LockPlayerControls(bool state)
    {
        if (playerController != null)
            playerController.enabled = !state;

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
}