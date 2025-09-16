using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData")]
    public class AttackData : ScriptableObject
    {
        public string attackName = "Basic Attack";
        public int damage = 10;
        public float attackDuration = 0.3f; // In seconds
        public Vector3 hitboxSize = new Vector3(1f, 1f, 1f);
    }
}