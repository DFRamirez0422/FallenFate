using UnityEngine;

public class ElevationEntry : MonoBehaviour
{
    [SerializeField] private Collider2D[] m_MountainColliders;
    [SerializeField] private Collider2D[] m_BoundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Collider2D mountain in m_MountainColliders)
            {
                mountain.enabled = false;
            }
            foreach (Collider2D boundary in m_BoundaryColliders)
            {
                boundary.enabled = true;
            }

            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
        }
    }
}
