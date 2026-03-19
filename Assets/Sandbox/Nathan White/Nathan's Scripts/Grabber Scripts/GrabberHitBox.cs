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
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Hitboxs" && !animator.GetBool("Died") && animator.GetBool("Attacking"))
        {
            buttonmashScript.started = true;
        }
    }
}
