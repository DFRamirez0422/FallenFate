// This code controls a material mask for UI effects
// It updates the "_Cutoff" value so you can animate or reveal parts of the image 
// Works with RAWIMAGE components <--- IMPORTANT

using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MaskDriver : MonoBehaviour
{
    [Range(0f, 1f)] public float cutoff = 1f; // how much is visible (0 = hidden, 1 = shown)
    Material matInstance; // stores a copy of the material

    void Awake()
    {
        var image = GetComponent<RawImage>(); // get the RawImage component
        if (image)
        {
            // make a new copy of the material so it doesn’t affect the original
            matInstance = new Material(image.material);
            image.material = matInstance; // assign the new material
        }
    }

    void Update()
    {
        // update the "_Cutoff" value every frame
        if (matInstance)
            matInstance.SetFloat("_Cutoff", cutoff); // apply the cutoff to the material
    }
}
