using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float m_ShakeAmount = 0.2f;
    private Vector3 m_InitialPos;

    private void Awake()
    {
        m_InitialPos = transform.position;
    }

    public void PlayShake()
    {
        Vector2 offset = Random.insideUnitCircle * m_ShakeAmount;
        transform.position = m_InitialPos + new Vector3(offset.x, offset.y, 0f);
    }
}
