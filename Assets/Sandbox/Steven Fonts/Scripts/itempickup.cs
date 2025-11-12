using UnityEngine;
using UnityEngine.InputSystem;
using NPA_Health_Components;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public enum ItemType { Heal, DamageBoost, ShieldReflect }
    public enum HealMode { FlatAmount, PercentOfMax }

    [Header("Item Settings")]
    public ItemType type = ItemType.Heal;

    [Header("Heal Settings")]
    public HealMode healMode = HealMode.FlatAmount;
    [Tooltip("Flat heal amount or percentage (0–1) depending on mode.")]
    public float healValue = 25f;

    [Header("Damage Boost Settings")]
    public float boostMultiplier = 1.5f;
    public float boostDuration = 8f;

    [Header("Shield Settings")]
    public float shieldDuration = 10f;

    [Header("Input (Optional)")]
    [Tooltip("Assign your Player/Interact action if using the new Input System. Otherwise, 'E' will work by default.")]
    public InputActionReference interactAction;

    [Header("Interaction Settings")]
    public bool useTrigger = true;
    [Range(0.5f, 5f)] public float interactRadius = 2.0f;
    public bool showPrompt = true;
    public string promptText = "Press E to consume";

    private bool playerInRange;
    private Health cachedHealth;
    private CombatEffects cachedEffects;
    private Camera cam;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (!col) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Awake()
    {
        cam = Camera.main;
        if (useTrigger)
        {
            var col = GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }
    }

    void Update()
    {
        FindNearbyPlayer();
        if (!playerInRange || cachedHealth == null) return;

        bool pressed = false;
        if (interactAction != null && interactAction.action != null && interactAction.action.enabled)
            pressed |= interactAction.action.WasPressedThisFrame();
        if (Keyboard.current != null)
            pressed |= Keyboard.current.eKey.wasPressedThisFrame;

        if (pressed)
            Consume();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger || !other.CompareTag("Player")) return;
        CacheTargets(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (!useTrigger || !other.CompareTag("Player")) return;
        if (cachedHealth && other.gameObject == cachedHealth.gameObject)
        {
            cachedHealth = null;
            cachedEffects = null;
            playerInRange = false;
        }
    }

    void FindNearbyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        playerInRange = false;
        cachedHealth = null;
        cachedEffects = null;

        if (players == null || players.Length == 0) return;

        float best = float.MaxValue;
        GameObject bestGo = null;
        Vector3 p = transform.position;
        foreach (var go in players)
        {
            float d = Vector3.Distance(p, go.transform.position);
            if (d < best && d <= interactRadius)
            {
                best = d;
                bestGo = go;
            }
        }
        if (bestGo)
            CacheTargets(bestGo);
    }

    void CacheTargets(GameObject go)
    {
        cachedHealth = go.GetComponent<Health>();
        cachedEffects = go.GetComponent<CombatEffects>();
        playerInRange = cachedHealth != null;
    }

    public void Consume()
    {
        if (!cachedHealth) return;

        switch (type)
        {
            case ItemType.Heal:
                if (healMode == HealMode.FlatAmount)
                    cachedHealth.HealAbsolute(Mathf.RoundToInt(Mathf.Max(0f, healValue)));
                else
                    cachedHealth.HealPercent(Mathf.Clamp01(healValue));
                break;

            case ItemType.DamageBoost:
                if (cachedEffects)
                    cachedEffects.ApplyDamageBoost(boostMultiplier, boostDuration);
                break;

            case ItemType.ShieldReflect:
                if (cachedEffects)
                    cachedEffects.ActivateShieldReflect(shieldDuration);
                break;
        }

        Destroy(gameObject);
    }

    void OnGUI()
    {
        if (!showPrompt || !playerInRange || cachedHealth == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(transform.position + Vector3.up * 0.6f);
        if (screen.z <= 0) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            normal = { textColor = Color.white }
        };

        GUI.Label(
            new Rect(screen.x - 90, Screen.height - screen.y - 20, 180, 40),
            promptText,
            style
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
