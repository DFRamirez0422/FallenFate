using UnityEngine;

public class Burn_items : MonoBehaviour
{
    public Animator MementosAnimator;

    public void burnItems()
    {
        MementosAnimator.SetBool("Burn_items", true);
    }
}
