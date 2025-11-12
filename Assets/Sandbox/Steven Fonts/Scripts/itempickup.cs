using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public enum ItemType { HealSmall, HealMedium, HealLarge, DamageBoost, ShieldReflect }

    [Header("Item")]
    public ItemType type = ItemType.HealSmall;

    [Tooltip("Used for Heal items")]
    public float healAmount = 25f;

    [Tooltip("Used for DamageBoost items")]
    public float boostMultiplier = 1.5f;
    public float boostDuration = 8f;

    [Tooltip("Used for Shield/Reflect items")]
    public float shieldDuration = 10f;

    [Header("Input (New Input System)")]
    public InputActionReference interactAction; // Assign your Player/Interact (E / Gamepad West)

    [Header("Interaction")]
    public bool useTrigger = true;        // If true, uses trigger enter/exit; otherwise proximity only
    public float interactRadius = 1.25f;  // Proximity fallback radius
    public bool showPrompt = true;
    public string promptText = "Press E to consume";

    private bool playerInRange;
    private PlayerStats cachedPlayer;
    private Camera cam;

    // -------------------- Setup & Lifecycle --------------------
    void Reset()
    {
        // Ensure we have a trigger collider + kinematic RB for reliable triggers
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Awake()
    {
        cam = Camera.main;

        // Safety: ensure a collider exists
        var col = GetComponent<Collider>();
        if (!col) col = gameObject.AddComponent<SphereCollider>();

        if (useTrigger && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"[ItemPickup:{name}] Collider set to Trigger for trigger-based interaction.");
        }
    }

    void OnEnable()
    {
        if (interactAction) interactAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactAction) interactAction.action.Disable();
    }

    // -------------------- Runtime --------------------
    void Update()
    {
        // Proximity fallback: if triggers are off OR we don't currently have a cached player, search nearby
        if (!useTrigger || cachedPlayer == null)
        {
            cachedPlayer = FindNearbyPlayer();
            playerInRange = cachedPlayer != null;
        }

        if (!playerInRange || cachedPlayer == null) return;

        // Accept new Input System action, or fall back to E if no action assigned
        bool pressed =
            (interactAction != null && interactAction.action.WasPressedThisFrame()) ||
            (!interactAction && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (pressed)
            Consume(cachedPlayer);
    }

    // Trigger path (recommended): item has kinematic RB + trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        if (!other.CompareTag("Player")) return;

        var ps = other.GetComponent<PlayerStats>();
        if (ps != null)
        {
            cachedPlayer = ps;
            playerInRange = true;
            // Debug.Log($"[ItemPickup:{name}] Player entered trigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!useTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (cachedPlayer != null && other.gameObject == cachedPlayer.gameObject)
        {
            cachedPlayer = null;
            playerInRange = false;
            // Debug.Log($"[ItemPickup:{name}] Player exited trigger");
        }
    }

    // -------------------- Helpers --------------------
    PlayerStats FindNearbyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players == null || players.Length == 0) return null;

        PlayerStats best = null;
        float bestD = float.MaxValue;
        Vector3 p = transform.position;

        foreach (var go in players)
        {
            var ps = go.GetComponent<PlayerStats>();
            if (!ps) continue;

            float d = Vector3.Distance(p, ps.transform.position);
            if (d < bestD && d <= interactRadius)
            {
                bestD = d;
                best = ps;
            }
        }
        return best;
    }

    void Consume(PlayerStats player)
    {
        if (!player) return;

        switch (type)
        {
            case ItemType.HealSmall:
            case ItemType.HealMedium:
            case ItemType.HealLarge:
                player.Heal(healAmount);
                Debug.Log($"[ItemPickup:{name}] Heal consumed (+{healAmount}).");
                break;

            case ItemType.DamageBoost:
                player.ApplyDamageBoost(boostMultiplier, boostDuration);
                Debug.Log($"[ItemPickup:{name}] Damage boost consumed (x{boostMultiplier} for {boostDuration}s).");
                break;

            case ItemType.ShieldReflect:
                player.ActivateShieldReflect(shieldDuration);
                Debug.Log($"[ItemPickup:{name}] Shield/Reflect consumed ({shieldDuration}s).");
                break;
        }

        Destroy(gameObject);
    }

    // Small world-space prompt without needing UI setup
    void OnGUI()
    {
        if (!showPrompt || !playerInRange || cachedPlayer == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(transform.position + Vector3.up * 0.6f);
        if (screen.z <= 0) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(screen.x - 90, Screen.height - screen.y - 20, 180, 40), promptText, style);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
