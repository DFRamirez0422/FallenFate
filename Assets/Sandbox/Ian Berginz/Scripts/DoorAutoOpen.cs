using UnityEngine;

public class DoorAutoOpen : MonoBehaviour
{
    public enum DoorOpenType { Slide, Rotate, DisableCollider }

    [Header("Door & Triggers")]
    public Transform doorObject;
    public Collider enemyCheckTrigger;
    public Collider playerExitTrigger;

    [Header("Enemy Detection")]
    public string[] enemyTags = new string[] { "Enemy" };

    [Header("Door Motion")]
    public DoorOpenType openType = DoorOpenType.Slide;
    public Vector3 slideOffset = new Vector3(0, 3, 0);
    public Vector3 rotateEulerAngles = new Vector3(0, 90, 0);
    public float openSpeed = 3f;
    public Collider doorCollider;

    [Header("Door Sounds")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private Transform player;
    private bool playerExited = false;

    private Vector3 closedPos;
    private Vector3 openPos;
    private Quaternion closedRot;
    private Quaternion openRot;

    private bool playerWasInsideLastFrame = false;
    private bool wasOpenLastFrame = false; // sound state tracking

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;

        closedPos = doorObject != null ? doorObject.localPosition : transform.localPosition;
        openPos = closedPos + slideOffset;

        closedRot = doorObject != null ? doorObject.localRotation : transform.localRotation;
        openRot = closedRot * Quaternion.Euler(rotateEulerAngles);

        if (enemyCheckTrigger == null)
            Debug.LogWarning($"{name}: enemyCheckTrigger not assigned.");
        if (playerExitTrigger == null)
            Debug.LogWarning($"{name}: playerExitTrigger not assigned.");
        if (doorObject == null)
            Debug.LogWarning($"{name}: doorObject not assigned.");
    }

    void Update()
    {
        bool enemiesPresent = AreEnemiesPresentInTrigger();

        float targetAlpha;
        if (playerExited)
            targetAlpha = 0f;
        else
            targetAlpha = enemiesPresent ? 0f : 1f;

        if (WasPlayerInsideAndNowOutside(playerExitTrigger))
            playerExited = true;

        ApplyDoorMotion(targetAlpha);
        HandleDoorSound(targetAlpha);
    }

    private void HandleDoorSound(float alpha)
    {
        bool isOpen = alpha >= 0.5f;

        if (isOpen != wasOpenLastFrame)
        {
            if (isOpen && openSound != null && audioSource != null)
                audioSource.PlayOneShot(openSound);

            if (!isOpen && closeSound != null && audioSource != null)
                audioSource.PlayOneShot(closeSound);

            wasOpenLastFrame = isOpen;
        }
    }

    private bool AreEnemiesPresentInTrigger()
    {
        if (enemyCheckTrigger == null) return false;
        var bounds = enemyCheckTrigger.bounds;
        Collider[] hits = Physics.OverlapBox(bounds.center, bounds.extents, enemyCheckTrigger.transform.rotation);

        foreach (var c in hits)
        {
            if (c == null) continue;
            foreach (var tag in enemyTags)
                if (!string.IsNullOrEmpty(tag) && c.CompareTag(tag))
                    return true;
        }
        return false;
    }

    private bool WasPlayerInsideAndNowOutside(Collider trigger)
    {
        if (trigger == null || player == null) return false;
        bool nowInside = trigger.bounds.Contains(player.position);
        bool result = playerWasInsideLastFrame && !nowInside;
        playerWasInsideLastFrame = nowInside;
        return result;
    }

    private void ApplyDoorMotion(float alpha)
    {
        if (doorObject == null) return;

        if (openType == DoorOpenType.Slide)
        {
            Vector3 desiredLocal = Vector3.Lerp(closedPos, openPos, alpha);
            doorObject.localPosition = Vector3.Lerp(doorObject.localPosition, desiredLocal, Time.deltaTime * openSpeed);
        }
        else if (openType == DoorOpenType.Rotate)
        {
            Quaternion desired = Quaternion.Slerp(closedRot, openRot, alpha);
            doorObject.localRotation = Quaternion.Slerp(doorObject.localRotation, desired, Time.deltaTime * openSpeed);
        }
        else if (openType == DoorOpenType.DisableCollider)
        {
            if (doorCollider != null)
                doorCollider.enabled = alpha < 0.5f;
        }
    }
}
