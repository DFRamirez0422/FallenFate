using UnityEngine;

public class PulseOnly : MonoBehaviour
{
    [SerializeField] private float pulseSize = 1.15f;   // The size of the pulse
    [SerializeField] private float returnSpeed = 5f;   // The speed of the return
    private Vector3 startSize; // Original scale of UI

    private void Start()
    {
        startSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
    }
    public void Pulse()
    {
        transform.localScale = startSize * pulseSize;
    }
}
