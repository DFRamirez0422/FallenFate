using UnityEngine;

public class QTEHolder : MonoBehaviour
{
    [Header("Key Binding")]
    public KeyCode assignedKey = KeyCode.W; // key for this lane

    [Header("Hit Window (UI units)")]
    public float hitWindowHalfHeight = 55f;  // timing window around holder center

    RectTransform rt; // cached rect

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public RectTransform Rect => rt; // expose rect
}
