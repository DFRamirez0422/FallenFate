using UnityEngine;
using NPA_Health_Components;

namespace NPA_PlayerPrefab.Scripts
{
    public class Hitbox : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private AttackData attackData; 
        private GameObject owner;

        public void Initialize(AttackData data, GameObject ownerObj)
        {
            attackData = data;
            owner = ownerObj;


            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null) col.size = attackData.hitboxSize;
            
            Destroy(gameObject, attackData.attackDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == owner) return; //dont hit self

            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackData.damage);
            }
        }
    }
}
