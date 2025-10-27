using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")] 
        [Tooltip("How fast the player moves")]
        [SerializeField] private float moveSpeed = 5f;

        [SerializeField] private float movementSlowOnAttack = 0.01f;
        
        [Tooltip("Small downward force to keep grounded")]
        [SerializeField] private float groundSnap = 2f;

        [Header("References")]
        [SerializeField] private Camera mainCamera;        // Main camera reference
        private CharacterController controller;            // CharacterController component
        
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

        [Header("Debug (ONLY FOR TESTING)")]
        [Tooltip("Text object to display the current player state.")]
        [SerializeField] private PlayerDebugUI m_DebugUI;
        
        // Internal dash state
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector3 dashDirection;
        
        // Dash Attack state
        private bool canDashAttack = false;      // Is player within dash attack window?
        private float dashAttackTimer = 0f;      // Counts down remaining time of dash attack window
        private bool dashAttackConsumed = false; // Was dash attack used during this window?
        
        // Exposed read-only property for PlayerCombat
        public bool DashAttackWindowActive => canDashAttack && !dashAttackConsumed;

        // Input and movement
        private Vector2 inputVector;             // Raw WASD input
        private Vector3 moveDirectionWorld;      // Final movement direction in world space
        private Vector3 lastFacingDirection = Vector3.right; // Default idle facing direction
        private Vector3 velocity;                // Current velocity applied
        public Vector3 FacingDirection =>
            moveDirectionWorld != Vector3.zero ? moveDirectionWorld : lastFacingDirection;
        private float attackForwardSpeed;       // Forward speed during attack
        
        private bool attackLocked = false;       // Locks movement when true
        public void SetAttackLock(bool value) => attackLocked = value;
        public void SetAttackSpeed(float value) => attackForwardSpeed = value;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (!mainCamera) mainCamera = Camera.main;
        }

        void Start()
        {
            // Lock and hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            ReadInput();                           // Step 1: Read WASD input
            GetMoveDirection();                    // Step 2: Convert to camera-relative world direction
            HandleDash(Time.deltaTime);
            HandleMovement(Time.deltaTime);        // Step 3: Calculate velocity
            ApplyMovement(Time.deltaTime);         // Step 4: Apply movement to CharacterController

            UpdateDebugUi(); // <--- TODO: remove when debugging code is finished
        }

        // Reads raw WASD input
        void ReadInput()
        {
            inputVector = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
        }

        // Converts 2D input into a 3D movement direction aligned to the camera's view
        void GetMoveDirection()
        {
            // Fallback: move relative to world if no camera exists
            if (mainCamera == null)
            {
                moveDirectionWorld = new Vector3(inputVector.x, 0, inputVector.y).normalized;
                return;
            }

            // Camera-relative axes
            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            // Get the camera's right direction, flattened
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Convert input (x = left/right, y = up/down) into world space using camera axes
            Vector3 direction = camRight * inputVector.x + camForward * inputVector.y;

            // Normalize to prevent faster diagonal movement
            moveDirectionWorld = direction.sqrMagnitude > 1f ? direction.normalized : direction;
            
            // Update last facing if moving
            if (moveDirectionWorld != Vector3.zero)
                lastFacingDirection = moveDirectionWorld;
        }

        // Sets velocity using calculated movement direction
        void HandleMovement(float dt)
        {
            if (isDashing)
            {
                velocity = dashDirection * dashSpeed;
            }
            else if (!attackLocked) // Only move if not attacking
            {
                velocity = moveDirectionWorld * moveSpeed;
            }
            else
            {
                velocity = moveDirectionWorld * moveSpeed * movementSlowOnAttack;
                // Apply the attack forward speed to movement.
                velocity += lastFacingDirection.normalized * attackForwardSpeed;

            }
        }

        // Applies velocity to the CharacterController (plus a small downward snap)
        void ApplyMovement(float dt)
        {
            if (!controller) return;
            
            Vector3 snapDown = Vector3.down * groundSnap;
            controller.Move((velocity + snapDown) * dt);
        }
        
        // Handles dash input, timers, and dash attack window
        void HandleDash(float dt)
        {
            // Check if dash was pressed this frame (SPACE/B)
            bool dashPressed = Input.GetKeyDown(KeyCode.Space) || 
                               Input.GetKeyDown(KeyCode.Joystick1Button1);

            // Reduce timers
            if (dashCooldownTimer > 0f) dashCooldownTimer -= dt;

            if (isDashing)
            {
                // Tick down active dash timer
                dashTimer -= dt;
                if (dashTimer <= 0f)
                    StopDash();
            }
            
            // Start dash
            if (dashPressed && !isDashing && dashCooldownTimer <= 0f
                && moveDirectionWorld != Vector3.zero)
            {
                StartDash();
            }
            
            // While dash attack window is active, tick down its timer
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
            dashTimer = dashDuration;           // Reset dash duration timer
            dashCooldownTimer = dashCooldown;   // Set cooldown before next dash
            dashDirection = moveDirectionWorld; // Lock direction at dash start
    
            // Open dash attack window
            canDashAttack = true;
            dashAttackConsumed  = false;        // Reset attack usage flag
            dashAttackTimer = dashAttackWindow; // Window countdown
            Debug.Log("Dash Started, Attack window open!");
        }

        void StopDash()
        {
            isDashing = false;
            dashDirection = Vector3.zero; // Clear direction
        }

        // Called by PlayerCombat once dash attack is executed
        public void ConsumeDashAttack()
        {
            dashAttackConsumed = true; // Mark that player used dash attack
            canDashAttack = false;     // Immediately close the window
        }
        
        //
        // ========================= DEBUG FUNCTIONS =========================
        //
        private void UpdateDebugUi()
        {
            // m_DebugUI.SetDebugPlayerSpeed($"{velocity.magnitude:f2}m/s");
            // if (dashCooldownTimer > 0f) m_DebugUI.SetDebugPlayerState($"Dash Cooldown : {dashCooldownTimer:f2}");
            // else if (moveDirectionWorld != Vector3.zero) m_DebugUI.SetDebugPlayerState("Moving");
            // else m_DebugUI.SetDebugPlayerState("Idle");

            // m_DebugUI.SetDebugPlayerSpeed($"{velocity.magnitude:f2}m/s");

            // Update debug UI based on movement and timer state.
            // if (dashCooldownTimer > 0f)
            // {
            //     m_DebugUI.SetDebugPlayerState($"Dash Cooldown : {dashCooldownTimer:f2}");
            // }
            // else if (moveDirectionWorld != Vector3.zero)
            // {
            //     m_DebugUI.SetDebugPlayerState("Moving");
            // }
            // else
            // {
            //     m_DebugUI.SetDebugPlayerState("Idle");
            // }

            // Update debug UI based on boolean flags.
            // if (dashAttackConsumed)
            // {
            //     m_DebugUI.SetDebugPlayerState("Dash Attack!");
            // }
            // else if (isDashing)
            // {
            //     m_DebugUI.SetDebugPlayerState("Dashing");
            // }
            // else if (attackLocked)
            // {
            //     m_DebugUI.SetDebugPlayerState("Attacking");
            // }
        }
    }
}