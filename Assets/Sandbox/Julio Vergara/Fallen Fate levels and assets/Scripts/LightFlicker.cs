using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light lightSource;   // Drag your light here
    public float minTime = 0.05f;
    public float maxTime = 0.25f;
    private float timer;

    void Start()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light>();

        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            lightSource.enabled = !lightSource.enabled;  // Toggle light on/off
            timer = Random.Range(minTime, maxTime);      // Randomize next flicker
        }
    }
}

