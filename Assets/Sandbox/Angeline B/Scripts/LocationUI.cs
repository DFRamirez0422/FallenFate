using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LocationUI : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.5f;
    public float displayTime = 2f;

    private Coroutine currentRoutine;

    private HashSet<string> visitedLocations = new HashSet<string>();

    public void ShowLocation(string locationName)
    {

        if (visitedLocations.Contains(locationName))
            return;

        visitedLocations.Add(locationName);

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(FadeRoutine(locationName));
    }

    IEnumerator FadeRoutine(string locationName)
    {
        locationText.text = "" + locationName;

        yield return StartCoroutine(Fade(0, 1));
        yield return new WaitForSeconds(displayTime);
        yield return StartCoroutine(Fade(1, 0));
    }

    IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = end;
    }
}