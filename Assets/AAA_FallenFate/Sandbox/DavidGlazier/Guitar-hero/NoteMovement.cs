using UnityEngine;

public class PerObjectMover : MonoBehaviour
{
    public enum MoveMode { None, FollowPlayer, WorldX, WorldY, WorldZ, AlongForward }

    [Header("Mode")]
    public MoveMode mode = MoveMode.WorldX;

    [Header("Common")]
    public float speed = 3f;

    [Header("Follow Player")]
    [Tooltip("If empty, will auto-find object tagged 'Player'.")]
    public Transform target;
    public bool rotateToFace = true;
    public float stopDistance = 0f;

    [Header("World/Forward (scroll)")]
    [Tooltip("+1 = forward/positive axis,  -1 = backward/negative axis")]
    public int direction = -1;

    // --- NEW: injected movement dir from spawner (world-space) ---
    [Header("Injected Movement")]
    [Tooltip("If true, AlongForward uses injectedDir instead of transform.forward.")]
    public bool useInjectedDirection = true;

    private Vector3 injectedDir = Vector3.zero;
    private Rigidbody rb;

    public void SetInjectedDirection(Vector3 worldDir)
    {
        injectedDir = worldDir.normalized;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (mode == MoveMode.FollowPlayer && target == null)
        {
            var obj = GameObject.FindWithTag("Player");
            if (obj) target = obj.transform;
        }
    }

    void Update()
    {
        switch (mode)
        {
            case MoveMode.FollowPlayer:
                if (!target) return;
                Vector3 toTarget = target.position - transform.position;

                if (stopDistance > 0f && toTarget.sqrMagnitude <= stopDistance * stopDistance)
                    return;

                {
                    Vector3 dir = toTarget.normalized;
                    Move(dir);
                    if (rotateToFace && dir.sqrMagnitude > 0.0001f)
                    {
                        Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(
                            transform.rotation, look, 360f * Time.deltaTime);
                    }
                }
                break;

            case MoveMode.WorldX:
                Move(Vector3.right * Mathf.Sign(direction));
                break;

            case MoveMode.WorldY:
                Move(Vector3.up * Mathf.Sign(direction));
                break;

            case MoveMode.WorldZ:
                Move(Vector3.forward * Mathf.Sign(direction));
                break;

            case MoveMode.AlongForward:
            {
                Vector3 dir = (useInjectedDirection && injectedDir != Vector3.zero)
                              ? injectedDir
                              : transform.forward;
                dir *= Mathf.Sign(direction);
                Move(dir);
                break;
            }

            case MoveMode.None:
            default:
                break;
        }
    }

    private void Move(Vector3 worldDir)
    {
        worldDir = worldDir.normalized;
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = worldDir * speed; // physics-friendly
        }
        else
        {
            transform.position += worldDir * speed * Time.deltaTime;
        }
    }
}
