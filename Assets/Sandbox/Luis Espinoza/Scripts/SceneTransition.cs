// This code handles scene transitions using an Animator
// When the button is clicked, it plays the close animation then loads the next scene
// Make sure the closeTime matches the TransitionClose animation length

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transition; // animator that controls the transition mask
    public string nextScene; // name of the next scene to load
    public float closeTime = 1f; // length of the close animation

    public void PlayClose() // called when pressing the transition button
    {
        StartCoroutine(DoTransition()); // start the transition process
    }

    IEnumerator DoTransition()
    {
        // play the close animation from the start
        transition.Play("TransitionClose", 0, 0f);

        // wait until the animation finishes
        yield return new WaitForSecondsRealtime(closeTime);

        // load the next scene
        SceneManager.LoadScene(nextScene);
    }
}
