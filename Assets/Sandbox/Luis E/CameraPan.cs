using UnityEngine;

public class MenuCameraPan : MonoBehaviour
{
    [Header("Pan Strength")]
    public float maxPanX = 0.5f;
    public float maxPanY = 0.3f;

    [Header("Smoothness")]
    public float smoothSpeed = 5f;

    [Header("Center Dead Zone")]
    [Range(0f, 0.49f)]
    public float deadZone = 0.25f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float mouseX = Input.mousePosition.x / Screen.width;
        float mouseY = Input.mousePosition.y / Screen.height;

        float x = GetEdgePan(mouseX);
        float y = GetEdgePan(mouseY);

        targetPos = startPos + new Vector3(x * maxPanX, y * maxPanY, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    float GetEdgePan(float value)
    {
        float centered = (value - 0.5f) * 2f;

        if (Mathf.Abs(centered) < deadZone)
            return 0f;

        float sign = Mathf.Sign(centered);
        float absValue = Mathf.Abs(centered);

        float scaled = Mathf.InverseLerp(deadZone, 1f, absValue);
        return scaled * sign;
    }
}