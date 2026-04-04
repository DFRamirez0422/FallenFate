using UnityEngine;
using System.Collections;

public class AutoSaveUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public void ShowAutoSave()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(5f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - t;
            yield return null;
        }
    }
}