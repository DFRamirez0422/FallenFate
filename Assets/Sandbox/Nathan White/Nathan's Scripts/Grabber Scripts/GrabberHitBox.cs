using UnityEngine;

public class GrabberHitBox : MonoBehaviour
{
    private ButtonMash buttonmashScript;

    private void Start()
    {
        // Finds the ButtonMash script on this object or any parent
        buttonmashScript = GetComponentInParent<ButtonMash>();

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            buttonmashScript.started = true;
        }
    }
}
