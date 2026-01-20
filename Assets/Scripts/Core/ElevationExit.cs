using UnityEngine;

public class ElevationExit : MonoBehaviour
{
    [SerializeField] private Collider2D[] m_MountainColliders;
    [SerializeField] private Collider2D[] m_BoundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Collider2D mountain in m_MountainColliders)
            {
                mountain.enabled = true;
            }
            foreach (Collider2D boundary in m_BoundaryColliders)
            {
                boundary.enabled = false;
            }

            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }
}
