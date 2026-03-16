using UnityEngine;

public class GrabberHitBox : MonoBehaviour
{
    private ButtonMash buttonmashScript;
    private Animator animator;

    private void Start()
    {
        // Finds the ButtonMash script on this object or any parent
        buttonmashScript = GetComponentInParent<ButtonMash>();
        animator = GetComponentInParent<Animator>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !animator.GetBool("Died") && animator.GetBool("Attacking"))
        {
            buttonmashScript.started = true;
        }
    }
}
