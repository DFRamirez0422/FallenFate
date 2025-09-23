using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData")]
    public class AttackData : ScriptableObject
    {
        [Header("Stats")]
        [Tooltip("Name of attack")]
        public string attackName = "Basic Attack";
        
        [Tooltip("How much damage attack deals")]
        public int damage = 10;
        
        [Header("Timing (seconds)")]
        [Tooltip("Time before the hitbox becomes active")]
        public float startupTime = 0.1f;

        [Tooltip("Time the hitbox is active and can hit enemies")]
        public float activeTime = 0.15f;

        [Tooltip("Time after active frames until player can act again")]
        public float recoveryTime = 0.2f;

        // Total duration for convenience
        public float TotalDuration => startupTime + activeTime + recoveryTime;

        
        [Header("Size")]
        public Vector3 hitboxSize = new Vector3(1f, 1f, 1f);
        
        [Header("Offsets")]
        public Vector3 hitboxOffset;
        public Vector3 rotationOffset = new Vector3(0f, -90f, 0f);
        
        [Header("Cooldown")]
        public float cooldown = 0.5f;
    }
}