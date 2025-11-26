using UnityEngine;

public class HeartAnimator : MonoBehaviour
{
    public Animator heartAnimator;

    void Start()
    {
        heartAnimator = GetComponent<Animator>(); // If the script is on the same object as the Animator
        AudioProcessor processor = FindObjectOfType<AudioProcessor> ();
		processor.onBeat.AddListener (onOnbeatDetected);
    }

    void onOnbeatDetected()
    {
        heartAnimator.SetTrigger("Pump");
    }

    void Update()
    {

    }
}
