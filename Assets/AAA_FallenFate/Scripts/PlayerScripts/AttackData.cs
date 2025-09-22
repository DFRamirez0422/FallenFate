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
        
        [Tooltip("How long attack lasts (seconds)")]
        public float attackDuration = 0.3f;
        
        [Header("Size")]
        public Vector3 hitboxSize = new Vector3(1f, 1f, 1f);
        
        [Header("Offsets")]
        public Vector3 hitboxOffset;
        public Vector3 rotationOffset = new Vector3(0f, -90f, 0f);
    }
}