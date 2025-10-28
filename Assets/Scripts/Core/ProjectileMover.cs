using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class ProjectileMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 10f;
        public Vector3 direction = Vector3.forward;

        [Header("Boomerang Settings")]
        [Tooltip("Enable to make this projectile reverse and come back.")]
        public bool boomerang = false;

        [Tooltip("Time before it starts returning to sender.")]
        public float forwardDuration = 0.8f;

        [Tooltip("Return speed multiplier (relative to forward speed).")]
        public float returnSpeedMult = 1.2f;

        [Tooltip("Automatically destroy projectile after return completes.")]
        public bool autoDestroyAfterReturn = true;

        [Header("Optional Animation")]
        [Tooltip("Optional Animator for visual effects (spin, glow, etc.).")]
        [SerializeField] private Animator projectileAnimator;

        private Transform owner;         // Optional: the object that fired it
        private float timer;
        private bool returning = false;
        private Vector3 startPos;
        private Vector3 returnTarget;

        public void SetOwner(Transform ownerTransform)
        {
            owner = ownerTransform;
            returnTarget = owner ? owner.position : transform.position - direction;
        }

        void Start()
        {
            startPos = transform.position;

            // Trigger launch animation if Animator is present
            if (projectileAnimator)
                projectileAnimator.SetTrigger("Launch");
        }

        void Update()
        {
            timer += Time.deltaTime;

            // Simple forward/return logic
            if (boomerang)
            {
                if (!returning && timer >= forwardDuration)
                {
                    returning = true;
                    timer = 0f;

                    // Trigger return animation if Animator is present
                    if (projectileAnimator)
                        projectileAnimator.SetTrigger("Return");
                }

                if (!returning)
                {
                    transform.position += direction.normalized * (speed * Time.deltaTime);
                }
                else
                {
                    // Return toward start or owner
                    Vector3 target = owner ? owner.position : startPos;
                    Vector3 toTarget = (target - transform.position).normalized;
                    transform.position += toTarget * (speed * returnSpeedMult * Time.deltaTime);

                    // When close enough, destroy or stop
                    if (Vector3.Distance(transform.position, target) < 0.3f)
                    {
                        if (projectileAnimator)
                            projectileAnimator.SetTrigger("Arrived");

                        if (autoDestroyAfterReturn)
                            Destroy(gameObject);
                        else
                            enabled = false;
                    }
                }
            }
            else
            {
                // Default straight projectile
                transform.position += direction.normalized * (speed * Time.deltaTime);
            }
        }
    }
}
