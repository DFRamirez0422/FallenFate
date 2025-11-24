using UnityEngine;

public class DoorManualOpen : MonoBehaviour
{
    public enum DoorOpenType { Slide, Rotate, DisableCollider }

    [Header("Door & Triggers")]
    public Transform doorObject;
    public Collider enemyCheckTrigger;
    public Collider playerExitTrigger;

    [Header("Enemy Detection")]
    public string[] enemyTags = new string[] { "Enemy" };

    [Header("Player Interaction")]
    public KeyCode openKey = KeyCode.E;
    public float interactDistance = 2f;

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

    private bool isUnlocked = false;
    private bool isOpening = false;
    private bool playerExited = false;

    private Vector3 closedPos;
    private Vector3 openPos;
    private Quaternion closedRot;
    private Quaternion openRot;

    private bool wasInsideExitLastFrame = false;
    private bool wasOpenLastFrame = false; // sound tracking

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        closedPos = doorObject.localPosition;
        openPos = doorObject.localPosition + slideOffset;

        closedRot = doorObject.localRotation;
        openRot = closedRot * Quaternion.Euler(rotateEulerAngles);
    }

    void Update()
    {
        isUnlocked = !AreEnemiesPresentInTrigger();

        if (isUnlocked && !playerExited && PlayerIsClose() && Input.GetKeyDown(openKey))
            isOpening = true;

        CheckPlayerExitTrigger();

        float alpha = playerExited ? 0f : (isOpening ? 1f : 0f);

        ApplyDoorMotion(alpha);
        HandleDoorSound(alpha);
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

    private bool PlayerIsClose()
    {
        if (player == null) return false;
        return Vector3.Distance(player.position, doorObject.position) <= interactDistance;
    }

    private bool AreEnemiesPresentInTrigger()
    {
        if (enemyCheckTrigger == null) return false;

        var bounds = enemyCheckTrigger.bounds;
        Collider[] hits = Physics.OverlapBox(bounds.center, bounds.extents, enemyCheckTrigger.transform.rotation);

        foreach (var col in hits)
        {
            foreach (var tag in enemyTags)
                if (col.CompareTag(tag))
                    return true;
        }

        return false;
    }

    private void CheckPlayerExitTrigger()
    {
        if (playerExitTrigger == null || player == null) return;

        bool inside = playerExitTrigger.bounds.Contains(player.position);

        if (wasInsideExitLastFrame && !inside)
            playerExited = true;

        wasInsideExitLastFrame = inside;
    }

    private void ApplyDoorMotion(float alpha)
    {
        if (openType == DoorOpenType.Slide)
        {
            Vector3 target = Vector3.Lerp(closedPos, openPos, alpha);
            doorObject.localPosition = Vector3.Lerp(doorObject.localPosition, target, Time.deltaTime * openSpeed);
        }
        else if (openType == DoorOpenType.Rotate)
        {
            Quaternion target = Quaternion.Slerp(closedRot, openRot, alpha);
            doorObject.localRotation = Quaternion.Slerp(doorObject.localRotation, target, Time.deltaTime * openSpeed);
        }
        else if (openType == DoorOpenType.DisableCollider)
        {
            if (doorCollider != null)
                doorCollider.enabled = alpha < 0.5f;
        }
    }
}
