using UnityEngine;

public class AspectRatioLock : MonoBehaviour
{
    [Header("Camera Settings")]
    public float targetAspect = 1920f / 1080f;

    [Header("UI Settings")]
    public RectTransform uiRoot; // Parent of all UI elements

    void Start()
    {
        LockCameraAspect();
        ClampUI();
    }

    void LockCameraAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        if (scaleHeight < 1.0f) // Black bars top/bottom
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else // Black bars left/right
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

    void ClampUI()
    {
        if (uiRoot == null) return;

        // Reset position to center
        uiRoot.localPosition = Vector3.zero;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        float scaleFactor = (scaleHeight < 1f) ? scaleHeight : 1f / scaleHeight;

        // Apply scale without moving the UI out of center
        uiRoot.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}
