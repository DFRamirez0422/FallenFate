using UnityEngine;

namespace Player
{
    public class PlayerControllerNPA : MonoBehaviour
    {
        [Header("Movement Settings")] 
        [Tooltip("How fast the player moves")]
        [SerializeField] private float moveSpeed = 5f;   
        
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
        
        // Internal dash state
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector3 dashDirection;

        // Input and movement
        private Vector2 inputVector;           // Raw WASD input
        private Vector3 moveDirectionWorld;    // Final movement direction in world space
        private Vector3 velocity;              // Velocity to apply this frame

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

            // Get the camera's forward direction, flattened (no vertical tilt)
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
        }

        // Sets velocity using calculated movement direction
        void HandleMovement(float dt)
        {
            if (isDashing)
            {
                velocity = dashDirection * dashSpeed;
            }
            else
            {
                velocity = moveDirectionWorld * moveSpeed;
            }
        }

        // Applies velocity to the CharacterController (plus a small downward snap)
        void ApplyMovement(float dt)
        {
            if (!controller) return;
            
            Vector3 snapDown = Vector3.down * groundSnap;
            controller.Move((velocity + snapDown) * dt);
        }
        
        void HandleDash(float dt)
        {
            // Check if dash was pressed this frame (SPACE/B)
            bool dashPressed = Input.GetKeyDown(KeyCode.Space) || 
                               Input.GetKeyDown(KeyCode.Joystick1Button1); 
            
            // Reduce timers
            if (dashCooldownTimer > 0f) dashCooldownTimer -= dt;

            if (isDashing)
            {
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
        }

        void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            dashDirection = moveDirectionWorld;
        }

        void StopDash()
        {
            isDashing = false;
            dashDirection = Vector3.zero;
        }
    }
}