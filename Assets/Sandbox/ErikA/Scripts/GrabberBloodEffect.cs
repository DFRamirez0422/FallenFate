using UnityEngine;

public class GrabberBloodEffect : MonoBehaviour
{
    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private Animator m_ImpactEffectAnimator;
    [SerializeField] private string[] m_ImpactStateNames;

    [SerializeField] private Transform m_LeftPoint;
    [SerializeField] private Transform m_RightPoint;
    [SerializeField] private Transform m_UpPoint;
    [SerializeField] private Transform m_DownPoint;

    private Animator m_GrabberAnimator;
    private Vector3 m_OriginalLocalScale;

    private void Awake()
    {
        m_GrabberAnimator = GetComponent<Animator>();

        if (m_ImpactEffect != null)
        {
            m_OriginalLocalScale = m_ImpactEffect.transform.localScale;

            if (m_ImpactEffectAnimator == null)
                m_ImpactEffectAnimator = m_ImpactEffect.GetComponentInChildren<Animator>(true);

            m_ImpactEffect.SetActive(false);
        }
    }

    public void PlayGrabbedImpact()
    {
        if (m_ImpactEffect == null || m_ImpactEffectAnimator == null || m_GrabberAnimator == null)
            return;

        Transform spawnPoint = GetSpawnPoint();
        if (spawnPoint == null)
            return;

        m_ImpactEffect.transform.position = spawnPoint.position;

        Vector3 newScale = m_OriginalLocalScale;
        float dirX = m_GrabberAnimator.GetFloat("DirX");
        float dirY = m_GrabberAnimator.GetFloat("DirY");

        if (Mathf.Abs(dirX) > Mathf.Abs(dirY))
        {
            if (dirX < 0f)
                newScale.x = -Mathf.Abs(m_OriginalLocalScale.x);
            else
                newScale.x = Mathf.Abs(m_OriginalLocalScale.x);
        }
        else
        {
            newScale.x = Mathf.Abs(m_OriginalLocalScale.x);
        }

        m_ImpactEffect.transform.localScale = newScale;
        m_ImpactEffect.SetActive(true);

        if (m_ImpactStateNames != null && m_ImpactStateNames.Length > 0)
        {
            int randomIndex = Random.Range(0, m_ImpactStateNames.Length);
            m_ImpactEffectAnimator.Play(m_ImpactStateNames[randomIndex], 0, 0f);
        }
        else
        {
            m_ImpactEffectAnimator.Play(0, 0, 0f);
        }

        m_ImpactEffectAnimator.Update(0f);
    }

    private Transform GetSpawnPoint()
    {
        float dirX = m_GrabberAnimator.GetFloat("DirX");
        float dirY = m_GrabberAnimator.GetFloat("DirY");

        if (Mathf.Abs(dirX) > Mathf.Abs(dirY))
            return dirX >= 0f ? m_RightPoint : m_LeftPoint;

        return dirY >= 0f ? m_UpPoint : m_DownPoint;
    }
}
