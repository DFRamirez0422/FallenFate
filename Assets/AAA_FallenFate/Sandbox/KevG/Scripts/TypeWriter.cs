using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriter : MonoBehaviour
{
    public float delay = 0.1f;            // Delay between each character
    public string fullText;               // The complete text to display
    private string currentText = "";      // The text being typed
    public GameObject DialogueBox;        // Reference to the dialogue box

    void Start()
    {
        StartCoroutine(ShowText());
        StartCoroutine(DeactivateAfterTime(10f));
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            GetComponent<Text>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator DeactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false); // Deactivate dialogue box
        }
        else
        {
            Debug.LogWarning("DialogueBox is not assigned!");
        }
    }
}
