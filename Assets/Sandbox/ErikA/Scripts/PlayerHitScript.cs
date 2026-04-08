using UnityEngine;

public class PlayerHitScript : MonoBehaviour
{
    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private Animator m_ImpactEffectAnimator;
    [SerializeField] private string[] m_ImpactStateNames;

    [SerializeField] private float m_HorizontalOffset = 0.25f;
    [SerializeField] private float m_GrabbedHorizontalOffset = 0.05f;

    
    
    private PlayerHealth m_PlayerHealth;
    private Vector3 m_OriginalLocalScale;
    private Vector3 m_OriginalLocalPosition;

    private void Awake()
    {
        m_PlayerHealth = GetComponent<PlayerHealth>();
        

        if (m_ImpactEffect != null)
        {
            m_OriginalLocalScale = m_ImpactEffect.transform.localScale;
            m_OriginalLocalPosition = m_ImpactEffect.transform.localPosition;

            if (m_ImpactEffectAnimator == null)
                m_ImpactEffectAnimator = m_ImpactEffect.GetComponentInChildren<Animator>(true);

            m_ImpactEffect.SetActive(false);
        }
    }

    public void ImpactEffect()
    {
        if (m_ImpactEffect == null || m_ImpactEffectAnimator == null) return;

        m_ImpactEffect.SetActive(true);

        Transform attacker = m_PlayerHealth != null ? m_PlayerHealth.LastHitSource : null;

        Vector3 newScale = m_OriginalLocalScale;
        Vector3 newPosition = m_OriginalLocalPosition;
        
        bool isGrabbed = GrabberMovement.SomeoneGrabbedPlayer;
        
        float offsetToUse = isGrabbed
            ? Mathf.Abs(m_GrabbedHorizontalOffset)
            : Mathf.Abs(m_HorizontalOffset);

        if (attacker != null)
        {
            Vector3 directionFromAttacker = transform.position - attacker.position;

            // attacker on left -> spray right
            if (directionFromAttacker.x >= 0f)
            {
                newScale.x = Mathf.Abs(m_OriginalLocalScale.x);
                newPosition.x = m_OriginalLocalPosition.x + Mathf.Abs(offsetToUse);
            }
            else
            {
                newScale.x = -Mathf.Abs(m_OriginalLocalScale.x);
                newPosition.x = m_OriginalLocalPosition.x - Mathf.Abs(offsetToUse);
            }
        }

        m_ImpactEffect.transform.localScale = newScale;
        m_ImpactEffect.transform.localPosition = newPosition;

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
}