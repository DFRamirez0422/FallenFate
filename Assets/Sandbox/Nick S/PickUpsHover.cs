using UnityEngine;

public class PickupsHover : MonoBehaviour
{
    [Header("Hover Movement")]
    [Tooltip("How far up and down the pickup moves from its starting position.")]
    [SerializeField] private float hoverHeight = 0.08f;

    [Tooltip("How fast the pickup bobs up and down.")]
    [SerializeField] private float hoverSpeed = 1.4f;

    [Header("Optional Flutter")]
    [Tooltip("Small side-to-side movement to make the hover feel less rigid.")]
    [SerializeField] private bool enableFlutter = true;

    [Tooltip("How far left and right the pickup drifts.")]
    [SerializeField] private float flutterWidth = 0.025f;

    [Tooltip("How fast the pickup drifts left and right.")]
    [SerializeField] private float flutterSpeed = 2.1f;

    [Header("Timing Offset")]
    [Tooltip("Offsets the motion so multiple pickups do not all move in sync.")]
    [SerializeField] private float phaseOffset = 0f;

    private Vector3 startLocalPosition;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;

        if (Mathf.Approximately(phaseOffset, 0f))
        {
            phaseOffset = Random.Range(0f, 100f);
        }
    }

    private void OnEnable()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float time = Time.time + phaseOffset;

        float yOffset = Mathf.Sin(time * hoverSpeed) * hoverHeight;
        float xOffset = 0f;

        if (enableFlutter)
        {
            xOffset = Mathf.Sin(time * flutterSpeed) * flutterWidth;
        }

        transform.localPosition = startLocalPosition + new Vector3(xOffset, yOffset, 0f);
    }
}