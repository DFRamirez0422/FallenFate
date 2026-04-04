using System.Collections;
using TMPro;
using UnityEngine;

public class TriggerTextFade : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Timing")]
    public float fadeDuration = 1f;
    public float displayTime = 5f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeMessageRoutine(message));
    }

    private IEnumerator FadeMessageRoutine(string message)
    {
        messageText.text = message;

        yield return StartCoroutine(FadeCanvas(0f, 1f));
        yield return new WaitForSeconds(displayTime);
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        gameObject.SetActive(false);
        currentRoutine = null;
    }

    private IEnumerator FadeCanvas(float startAlpha, float endAlpha)
    {
        float time = 0f;
        canvasGroup.alpha = startAlpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}