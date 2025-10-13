using UnityEngine;
using TMPro;
using System.Collections;

public class TextResizeAndSpaceAnimation : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    [Header("Timing")]
    public float scaleDuration = 0.6f;     // Half-cycle (expand or contract)
    [Header("Sizing")]
    public float maxScale = 1.15f;         // Subtle expansion factor
    public float maxLetterSpacing = 2f;    // Expanded spacing
    public float minLetterSpacing = 0f;    // Contracted spacing
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float baseFontSize;

    void Start()
    {
        if (textMeshPro != null)
        {
            baseFontSize = textMeshPro.fontSize;
            StartCoroutine(Pulse());
        }
    }

    IEnumerator Pulse()
    {
        float t = 0f;
        bool expanding = true;

        while (true)
        {
            // Normalized progress
            float normalized = t / scaleDuration;
            float eased = easing.Evaluate(normalized);

            // Calculate current values relative to the ORIGINAL size
            float scale = Mathf.Lerp(1f, maxScale, expanding ? eased : 1f - eased);
            textMeshPro.fontSize = baseFontSize * scale;

            float spacing = Mathf.Lerp(minLetterSpacing, maxLetterSpacing, expanding ? eased : 1f - eased);
            textMeshPro.characterSpacing = spacing;

            t += Time.deltaTime;
            if (t >= scaleDuration)
            {
                t = 0f;
                expanding = !expanding;
            }

            yield return null;
        }
    }
}

