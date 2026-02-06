using Unity.VisualScripting;
using UnityEngine;


public class EnemyHitScript : MonoBehaviour
{
    // Audio
    [SerializeField] private AudioClip m_HitSound;

    [SerializeField] private AudioSource m_EnemySoundSource;

    // hit fx
    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private Animator m_ImpactEffectAnimator;
    

    // enemy sprite for white flash
    [SerializeField] private SpriteRenderer m_EnemySprite; 
    [SerializeField] private Material m_FlashMaterial;
    [SerializeField] private float m_HitDuration = 0.2f;
    private Material m_OriginalMaterial;
    void Awake()
    {
        m_EnemySprite = GetComponent<SpriteRenderer>();
        m_EnemySoundSource = GetComponent<AudioSource>();
        m_OriginalMaterial = m_EnemySprite.material;
        m_ImpactEffectAnimator = m_ImpactEffect.GetComponent<Animator>();
        if(m_ImpactEffect) m_ImpactEffect.SetActive(false);
    }

    // Update is called once per frame
    public void PlayHitSound()
    {
        if (m_EnemySoundSource != null)
        {
            m_EnemySoundSource.PlayOneShot(m_HitSound);
        }
    }

    public void FlashWhite()
    {
        CancelInvoke(nameof(ResetSpriteMaterial));
        Material material = m_FlashMaterial;
        m_EnemySprite.material = material;
        Invoke(nameof(ResetSpriteMaterial), m_HitDuration);
        

    }

    void ResetSpriteMaterial()
    { 
        m_EnemySprite.material = m_OriginalMaterial;
    }
    
    public void ImpactEffect()
    {
        
        if (m_ImpactEffect != null)
        {
            if (!m_ImpactEffect || !m_ImpactEffectAnimator) return;
             m_ImpactEffect.SetActive(true);
             m_ImpactEffectAnimator.Play(0, 0, 0f); 
             m_ImpactEffectAnimator.Update(0f);
        }
    }
    

    
}