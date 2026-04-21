using UnityEngine;

public class Burn_items : MonoBehaviour
{
    public Animator MementosAnimator;
    public SceneLoadCountdownTimer Timer;

    public void burnItems()
    {
        MementosAnimator.SetBool("Burn_items", true);
        Timer.enabled = true;
    }
}
