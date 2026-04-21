using System.Collections;
using UnityEngine;

public class Burn_items : MonoBehaviour
{
    public Animator MementosAnimator;

    [Tooltip("Shown after the burn clip on the animator finishes (state Mementos_Burn).")]
    public GameObject objectToEnableAfterBurn;

    [Tooltip("Animator state that plays when Burn_items is true. Must match the controller state name.")]
    public string burnAnimatorStateName = "Mementos_Burn";

    public void burnItems()
    {
        MementosAnimator.SetBool("Burn_items", true);
    }

    public void endScene()
    {
        if (MementosAnimator == null || objectToEnableAfterBurn == null)
            return;

        StopCoroutine(nameof(WaitForBurnAnimationThenEnable));
        StartCoroutine(WaitForBurnAnimationThenEnable());
    }

    private IEnumerator WaitForBurnAnimationThenEnable()
    {
        const float maxWaitSeconds = 120f;
        float elapsed = 0f;

        while (elapsed < maxWaitSeconds)
        {
            AnimatorStateInfo state = MementosAnimator.GetCurrentAnimatorStateInfo(0);
            bool inBurnState = state.IsName(burnAnimatorStateName);
            bool finished = inBurnState && state.normalizedTime >= 1f && !MementosAnimator.IsInTransition(0);

            if (finished)
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= maxWaitSeconds)
        {
            Debug.LogWarning($"{nameof(Burn_items)}: timed out waiting for '{burnAnimatorStateName}' to finish.");
            yield break;
        }

        objectToEnableAfterBurn.SetActive(true);
    }
}
