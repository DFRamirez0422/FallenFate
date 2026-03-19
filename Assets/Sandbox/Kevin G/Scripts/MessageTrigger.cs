using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public TriggerTextFade textUI;
    [TextArea(2, 5)]
    public string message;

    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if (other.CompareTag("Player"))
        {
            textUI.ShowMessage(message);
            hasTriggered = true;
        }
    }
}