using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingDialogue : MonoBehaviour
{
    [Header("Floating Text Settings")]
    [Tooltip("The prefab to display text above the NPC or object.")]
    public GameObject floatingTextPrefab;

    [Tooltip("Offset position relative to the transform.")]
    public Vector3 textOffset = new Vector3(0f, 1f, 0f);

    [Tooltip("How long the text stays visible after typing finishes.")]
    public float displayTime = 3f;

    [Header("Typing Settings")]
    [Tooltip("0 = instant, 1 = very slow. Recommended default = 0.5.")]
    [Range(0f, 1f)]
    public float typingSpeed = 0.5f;

    private GameObject currentTextInstance;
    private Coroutine typingCoroutine;

    public void ShowFloatingLine(string text)
    {
        if (floatingTextPrefab == null || string.IsNullOrEmpty(text))
            return;

        // Destroy any existing text first
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
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(tmp, text));
        }

        Billboard billboard = currentTextInstance.GetComponent<Billboard>();
        if (billboard == null)
            currentTextInstance.AddComponent<Billboard>();

        // Destroy after full display duration
        Destroy(currentTextInstance, displayTime + text.Length * 0.02f);
    }

    private IEnumerator TypeText(TextMeshPro tmp, string fullText)
    {
        tmp.text = "";

        // Convert slider (0–1) into actual typing delay
        // 0 = instant, 1 = slowest (0.05s per char), 0.5 = balanced (~0.02s)
        float delay = Mathf.Lerp(0f, 0.05f, typingSpeed);

        // If speed = 0, skip typewriter effect
        if (delay <= 0f)
        {
            tmp.text = fullText;
            yield break;
        }

        foreach (char c in fullText)
        {
            tmp.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}
