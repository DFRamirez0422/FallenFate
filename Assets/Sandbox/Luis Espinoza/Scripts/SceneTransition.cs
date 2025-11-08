using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transition;      // Animator on TransitionMask
    public string nextScene;
    public float closeTime = 1f;   // EXACT length of TransitionClose clip

    public void PlayClose()          // hook this to the Button OnClick
    {
        StartCoroutine(DoTransition());
    }

    IEnumerator DoTransition()
    {
        // Play the state immediately on Base Layer (0), at normalized time 0
        transition.Play("TransitionClose", 0, 0f);

        yield return new WaitForSecondsRealtime(closeTime);
        SceneManager.LoadScene(nextScene);
    }
}
