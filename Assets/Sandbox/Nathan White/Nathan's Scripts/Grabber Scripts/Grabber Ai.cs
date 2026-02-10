using UnityEngine;

public class GrabberAi : MonoBehaviour
{
    private ButtonMash buttonMash;

    private void Start()
    {
        buttonMash = GetComponent<ButtonMash>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        buttonMash.started = true;
    }
}
