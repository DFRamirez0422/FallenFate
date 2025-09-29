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

    private Queue<string> sentences;
    private Queue<Sprite> characterSprites;
    private bool isTyping = false; // Tracks if text is currently typing

    void Start()
    {
        sentences = new Queue<string>();
        characterSprites = new Queue<Sprite>();
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        // Advance dialogue when player presses Space
        if (dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Finish the current sentence instantly
                StopAllCoroutines();
                dialogueText.text = sentences.Peek(); // Show full sentence
                isTyping = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        sentences.Clear();
        characterSprites.Clear();

        foreach (string sentence in dialogue.sentences)
            sentences.Enqueue(sentence);

        foreach (Sprite sprite in dialogue.characterSprites)
            characterSprites.Enqueue(sprite);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Sprite sprite = characterSprites.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        characterImage.sprite = sprite;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // Or use WaitForSeconds(0.02f) for slower typing
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
    }
}