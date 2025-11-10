// This code forces the scene to play the open transition animation right away
// It makes sure the scene starts already opened without delay

using UnityEngine;

public class SceneOpen : MonoBehaviour
{
    public Animator transition; // animator that controls the transition mask

    void Awake()
    {
        // play the open animation instantly on scene start
        transition.Play("TransitionOpen", 0, 0f); // play the animation from the first frame
        transition.Update(0f); // apply the animation immediately
    }
}
