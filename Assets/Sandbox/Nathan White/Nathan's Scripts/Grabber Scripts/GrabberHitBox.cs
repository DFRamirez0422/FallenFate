using UnityEngine;

public class GrabberHitBox : MonoBehaviour
{
    private ButtonMash buttonmashScript;

    private void Start()
    {
        buttonmashScript = GetComponent<ButtonMash>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        buttonmashScript.started = true;
    }
}
