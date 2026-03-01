using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    public float height = 0.5f;   // how high it moves
    public float speed = 1f;      // how fast it moves

    Vector3 startPos;             // starting position
    float t;                      // time value we move through

    void Start()
    {
        startPos = transform.localPosition;  // save start
    }

    void Update()
    {
        // move time forward
        t += Time.deltaTime * speed;

        // make a smooth up-and-down number (0 → 1 → 0 → 1)
        float wave = Mathf.PingPong(t, 1f);

        // smooth the curve so it's soft at the ends
        float smoothWave = Mathf.SmoothStep(0f, 1f, wave);

        // apply to Y movement
        transform.localPosition = startPos + new Vector3(0, smoothWave * height, 0);
    }
}
