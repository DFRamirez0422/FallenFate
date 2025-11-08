using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MaskDriver : MonoBehaviour
{
    [Range(0f, 1f)] public float cutoff = 1f;
    Material matInstance;

    void Awake()
    {
        var image = GetComponent<RawImage>();
        if (image)
        {
            // Clone the material so it's safe to modify
            matInstance = new Material(image.material);
            image.material = matInstance;
        }
    }

    void Update()
    {
        if (matInstance)
            matInstance.SetFloat("_Cutoff", cutoff);
    }
}
