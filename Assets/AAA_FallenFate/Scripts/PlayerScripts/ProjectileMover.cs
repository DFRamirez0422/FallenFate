using UnityEngine;

namespace NPA_PlayerPrefab.Scripts
{
    public class ProjectileMover : MonoBehaviour
    {
        public float speed = 10f;
        public Vector3 direction = Vector3.forward;

        void Update()
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}