using UnityEngine;

public class SceneOpen : MonoBehaviour
{
    public Animator transition;                      // TransitionMask Animator

    void Awake()
    {
        // FORCE the Animator to spawn the open animation first frame
        transition.Play("TransitionOpen", 0, 0f);   // play state at time=0
        transition.Update(0f);                       // apply immediately
    }
}
