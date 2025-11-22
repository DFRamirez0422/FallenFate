using NUnit.Framework;
using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Base move speed")]
        [SerializeField] private float moveSpeed = 5f;

        [Tooltip("Multiplier to slow strafing during attack-lock")]
        [SerializeField] private float movementSlowOnAttack = 0.01f;

        [Tooltip("Small downward force to keep grounded")]
        [SerializeField] private float groundSnap = 2f;

        [Header("Snappy Feel")]
        [Tooltip("Ignore tiny inputs to avoid drift")]
        [SerializeField] private float inputDeadzone = 0.15f;

        [Tooltip("If true, clamp player depth (Z) between -laneWidth and +laneWidth")]
        [SerializeField] private bool clampZLane = true;

        [Tooltip("Half-width of your 2.5D lane (world-space Z)")]
        [SerializeField] private float laneWidth = 3.0f;

        [Header("References")]
        [SerializeField] private Camera mainCamera;      // Main camera
        private CharacterController controller;          // CharacterController

        [Header("Dash Settings")]
        [Tooltip("How fast the player dashes")]
        [SerializeField] private float dashSpeed = 15f;

        [Tooltip("How long the player dashes for")]
        [SerializeField] private float dashDuration = 0.5f;

        [Tooltip("How long before player can dash again")]
        [SerializeField] private float dashCooldown = 1f;

        [Header("Dash Attack Settings")]
        [Tooltip("Timing window to perform dash attack")]
        [SerializeField] private float dashAttackWindow = .5f;

        [Header("Facing / Visuals")]
        [Tooltip("Rotate the visual root to yaw toward facingDir (for meshes). If false and SpriteRenderer exists, we flipX instead).")]
        [SerializeField] private bool rotateVisualToFacing = true;

        [Tooltip("Optional visual root (mesh/pivot). If null, uses this transform.")]
        [SerializeField] private Transform visualRoot;

        [Tooltip("Optional explicit sprite (for 2D/quads). If null, will auto-find one.")]
        [SerializeField] private SpriteRenderer sprite;

        [Tooltip("Stun tint color (temporary visual while stunned)")]
        [SerializeField] private Color stunTint = Color.red;

        // vvvvv Added by Jose E. from original file. vvvvv //

        [Header("Debug (ONLY FOR TESTING)")]
        [Tooltip("Text object to display the current player state.")]
        [SerializeField] private PlayerDebugUI m_DebugUI;

        /// <summary>
        /// Exposed public variable to retrieve the current velocity.
        /// </summary>
        public float Velocity => desiredVelocity.magnitude;

        /// <summary>
        /// Exposed public variable to tell whether or not the player is currently in a dash.
        /// </summary>
        public bool IsDashing => isDashing || dashAttackConsumed;

        // ^^^^^ Added by Jose E. from original file. ^^^^^ //

        [Header("Hitstun Reference")]
        [SerializeField] private Hitstun hitstun; // Movement is disabled while stunned

        // -------- Added in by Angel Rodriguez --------
        // this is to stop movement while healing
        public bool IsHealing = false; //This is to stop movement while healing
        [Tooltip("Healing duration to stop movement")]
        public float healingTimer; //How long the healing stops movement
        float currentHealingTime;

        // Note: ParryBlock is NOT gated here; parry can still run independently.

        // -------- Internal State --------
        // Dash
        public bool isDashing = false;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector3 dashDirection;

        // Dash Attack
        private bool canDashAttack = false;
        private float dashAttackTimer = 0f;
        private bool dashAttackConsumed = false;
        public bool DashAttackWindowActive => canDashAttack && !dashAttackConsumed;

        // Input & movement
        private Vector2 inputVector;                // raw input
        private Vector3 moveDirectionWorld;         // camera-relative direction
        private Vector3 desiredVelocity;            // computed fresh each frame (snappy)

        // Real facing
        private Vector3 facingDir = Vector3.right;  // persistent, normalized
        public Vector3 FacingDirection => facingDir;

        // Attack lock integration (from your combat script)
        private bool attackLocked = false;
        private float attackForwardSpeed;
        public void SetAttackLock(bool value) => attackLocked = value;
        public void SetAttackSpeed(float value) => attackForwardSpeed = value;

        // Stun visuals
        private bool prevStunned = false;
        private Renderer[] renderers;
        private MaterialPropertyBlock mpb;
        private Color[] originalRendererColors;
        private Color originalSpriteColor = Color.white;

        // Convenience
        private bool IsMovementLocked => hitstun != null && hitstun.IsStunned;

        void Awake()
        {
            currentHealingTime = healingTimer;
            controller = GetComponent<CharacterController>();
            if (!mainCamera) mainCamera = Camera.main;

            if (!hitstun) hitstun = GetComponent<Hitstun>();

            // Visual discovery
            if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>(true);
            if (sprite) originalSpriteColor = sprite.color;

            renderers = GetComponentsInChildren<Renderer>(true);
            mpb = new MaterialPropertyBlock();
            originalRendererColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                var m = r ? r.sharedMaterial : null;
                if (m != null)
                {
                    if (m.HasProperty("_BaseColor")) originalRendererColors[i] = m.GetColor("_BaseColor");
                    else if (m.HasProperty("_Color")) originalRendererColors[i] = m.GetColor("_Color");
                    else originalRendererColors[i] = Color.white;
                }
                else originalRendererColors[i] = Color.white;
            }
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {

            float dt = Time.deltaTime;

            ReadInput();


            GetMoveDirection();

            // Maintain real facing from dash / movement
            UpdateFacing();

            // Stun visuals + movement gating (parry is NOT affected here)
            bool stunned = hitstun != null && hitstun.IsStunned;
            HandleStunVisualEdge(stunned);

            if (!stunned)
            {
                HandleDash(dt);          // dash disabled during stun
                HandleMovementSnappy(dt);
            }
            else
            {
                desiredVelocity = Vector3.zero; // hard stop while stunned
            }

            // -------- Added in by Angel Rodriguez --------
            // this is to stop movement while healing
            if (IsHealing)
            {
                desiredVelocity = Vector3.zero; // hard stop while healing
                currentHealingTime -= 1 * Time.deltaTime;
                Debug.Log("ISHEALING SET TO true " + currentHealingTime);
                if (currentHealingTime <= 0)
                {
                    IsHealing = false;
                    currentHealingTime = healingTimer;
                    Debug.Log("ISHEALING SET TO FALSE");
                }
            }
            
            ApplyMovement(dt);

            if (clampZLane) ClampLaneZ();

            UpdateDebugUi(); // <--- TODO: remove when debugging code is finished
        }

        // --- Input ---
        void ReadInput()
        {
            inputVector = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        // --- Camera-relative move vector (with deadzone) ---
        void GetMoveDirection()
        {
            if (inputVector.magnitude < inputDeadzone)
                inputVector = Vector2.zero;

            if (mainCamera == null)
            {
                moveDirectionWorld = new Vector3(inputVector.x, 0f, inputVector.y);
                if (moveDirectionWorld.sqrMagnitude > 1f) moveDirectionWorld.Normalize();
                return;
            }

            Vector3 camForward = mainCamera.transform.forward; camForward.y = 0f; camForward.Normalize();
            Vector3 camRight   = mainCamera.transform.right;   camRight.y = 0f;   camRight.Normalize();

            Vector3 dir = camRight * inputVector.x + camForward * inputVector.y;
            moveDirectionWorld = (dir.sqrMagnitude > 1f) ? dir.normalized : dir;
        }

        // --- Real facing maintenance ---
        void UpdateFacing()
        {
            if (isDashing && dashDirection != Vector3.zero)
            {
                facingDir = new Vector3(dashDirection.x, 0f, dashDirection.z).normalized;
            }
            else if (moveDirectionWorld != Vector3.zero)
            {
                facingDir = new Vector3(moveDirectionWorld.x, 0f, moveDirectionWorld.z).normalized;
            }

            // Apply to visuals (either rotate meshes or flip sprite)
            if (rotateVisualToFacing)
            {
                Transform t = visualRoot ? visualRoot : transform;
                if (facingDir.sqrMagnitude > 0.0001f)
                {
                    Quaternion look = Quaternion.LookRotation(new Vector3(facingDir.x, 0f, facingDir.z), Vector3.up);
                    // yaw only
                    t.rotation = Quaternion.Euler(0f, look.eulerAngles.y, 0f);
                }
            }
            else if (sprite != null)
            {
                // flip sprite left/right — classic brawler
                if (Mathf.Abs(facingDir.x) > 0.0001f)
                    sprite.flipX = (facingDir.x < 0f);
            }
        }

        // --- Snappy movement: no inertia. Compute from current state only. ---
        void HandleMovementSnappy(float dt)
        {
            // If stunned, we never get here (Update gates this), but keep guard for safety.
            if (IsMovementLocked)
            {
                desiredVelocity = Vector3.zero;
                return;
            }

            if (isDashing)
            {
                desiredVelocity = dashDirection * dashSpeed;
                return;
            }

            if (!attackLocked)
            {
                desiredVelocity = (moveDirectionWorld != Vector3.zero)
                    ? moveDirectionWorld * moveSpeed
                    : Vector3.zero;
            }
            else
            {
                Vector3 strafing = moveDirectionWorld * (moveSpeed * movementSlowOnAttack);
                Vector3 forward  = (facingDir.sqrMagnitude > 0f ? facingDir : Vector3.forward) * attackForwardSpeed;
                desiredVelocity  = strafing + forward;
            }
        }

        // --- Apply movement + small downward snap ---
        void ApplyMovement(float dt)
        {
            if (!controller) return;

            Vector3 snapDown = Vector3.down * groundSnap;
            controller.Move((desiredVelocity + snapDown) * dt);
        }

        // --- Dash logic + dash attack window ---
        void HandleDash(float dt)
        {
            bool dashPressed = Input.GetKeyDown(KeyCode.Space) ||
                               Input.GetKeyDown(KeyCode.Joystick1Button1);

            if (dashCooldownTimer > 0f) dashCooldownTimer -= dt;

            if (isDashing)
            {
                dashTimer -= dt;
                if (dashTimer <= 0f)
                    StopDash();
            }

            if (dashPressed && !isDashing && dashCooldownTimer <= 0f
                && moveDirectionWorld != Vector3.zero && !IsMovementLocked)
            {
                StartDash();
            }

            if (canDashAttack)
            {
                dashAttackTimer -= dt;
                if (dashAttackTimer <= 0f)
                {
                    if (!dashAttackConsumed)
                        Debug.Log("Dash Attack Window Missed!");
                    canDashAttack = false;
                }
            }
        }

        void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            dashDirection = (moveDirectionWorld != Vector3.zero) ? moveDirectionWorld : facingDir;

            // Open dash attack window
            canDashAttack = true;
            dashAttackConsumed = false;
            dashAttackTimer = dashAttackWindow;

            Debug.Log("Dash Started, Attack window open!");
        }

        void StopDash()
        {
            isDashing = false;
            dashDirection = Vector3.zero;
        }

        // Called by PlayerCombat once dash attack is executed
        public void ConsumeDashAttack()
        {
            dashAttackConsumed = true;
            canDashAttack = false;
        }

        // --- Keep player inside Z lane (optional) ---
        void ClampLaneZ()
        {
            Vector3 p = transform.position;
            p.z = Mathf.Clamp(p.z, -laneWidth, laneWidth);
            transform.position = p;
        }

        // --- Stun visuals (no adapter needed) ---
        void HandleStunVisualEdge(bool stunned)
        {
            if (stunned && !prevStunned) OnStunEnter();
            if (!stunned && prevStunned) OnStunExit();
            prevStunned = stunned;
        }

        void OnStunEnter()
        {
            // Mesh tint
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (!r) continue;
                r.GetPropertyBlock(mpb);
                var m = r.sharedMaterial;
                if (m)
                {
                    if (m.HasProperty("_BaseColor")) mpb.SetColor("_BaseColor", stunTint);
                    if (m.HasProperty("_Color"))     mpb.SetColor("_Color",     stunTint);
                }
                r.SetPropertyBlock(mpb);
            }

            // Sprite tint
            if (sprite) sprite.color = stunTint;
        }

        void OnStunExit()
        {
            // Restore meshes
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (!r) continue;
                r.GetPropertyBlock(mpb);
                var m = r.sharedMaterial;
                if (m)
                {
                    if (m.HasProperty("_BaseColor")) mpb.SetColor("_BaseColor", originalRendererColors[i]);
                    if (m.HasProperty("_Color"))     mpb.SetColor("_Color",     originalRendererColors[i]);
                }
                r.SetPropertyBlock(mpb);
            }

            // Restore sprite
            if (sprite) sprite.color = originalSpriteColor;
        }

        // ========================= DEBUG =========================
        
        private void UpdateDebugUi()
        {
            if (!m_DebugUI) return;

            m_DebugUI.SetDebugPlayerSpeed($"{Velocity:f2}m/s");

            // Update debug UI based on movement and timer state.
            if (dashCooldownTimer > 0f)
            {
                m_DebugUI.SetDebugPlayerState($"Dash Cooldown : {dashCooldownTimer:f2}");
            }
            else if (moveDirectionWorld != Vector3.zero)
            {
                m_DebugUI.SetDebugPlayerState("Moving");
            }
            else
            {
                m_DebugUI.SetDebugPlayerState("Idle");
            }

            // Update debug UI based on boolean flags.
            if (dashAttackConsumed)
            {
                m_DebugUI.SetDebugPlayerState("Dash Attack!");
            }
            else if (isDashing)
            {
                m_DebugUI.SetDebugPlayerState("Dashing");
            }
            else if (attackLocked)
            {
                m_DebugUI.SetDebugPlayerState("Attacking");
            }
        }
    }
}
