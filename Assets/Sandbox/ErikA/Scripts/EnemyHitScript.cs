using UnityEngine;

public class EnemyHitScript : MonoBehaviour
{
    [SerializeField] private SoundDefinition m_HitSound;
    [SerializeField] private AudioSource m_EnemySoundSource;

    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private Animator m_ImpactEffectAnimator;

    [SerializeField] private SpriteRenderer m_EnemySprite;
    [SerializeField] private Material m_FlashMaterial;
    [SerializeField] private float m_HitDuration = 0.2f;
    private Material m_OriginalMaterial;

    private void Awake()
    {
        if (m_EnemySprite == null) m_EnemySprite = GetComponent<SpriteRenderer>();
        if (m_EnemySoundSource == null) m_EnemySoundSource = GetComponent<AudioSource>();
        if (m_EnemySprite != null) m_OriginalMaterial = m_EnemySprite.material;
        if (m_ImpactEffect != null)
        {
            m_ImpactEffectAnimator = m_ImpactEffect.GetComponent<Animator>();
            m_ImpactEffect.SetActive(false);
        }
    }

    public void PlayHitSound()
    {
        // if (m_EnemySoundSource != null && m_HitSound != null)
           // m_EnemySoundSource.PlayOneShot(m_HitSound);
           
        // NEW method for playing hit sound effects using SoundFXManager script
        if (m_HitSound != null)
            SoundFXManager.instance.Play(m_HitSound, transform);
    }

    public void FlashWhite()
    {
        if (m_EnemySprite == null || m_FlashMaterial == null) return;
        CancelInvoke(nameof(ResetSpriteMaterial));
        m_EnemySprite.material = m_FlashMaterial;
        Invoke(nameof(ResetSpriteMaterial), m_HitDuration);
    }

    private void ResetSpriteMaterial()
    {
        if (m_EnemySprite != null && m_OriginalMaterial != null)
            m_EnemySprite.material = m_OriginalMaterial;
    }

    public void ImpactEffect()
    {
        if (m_ImpactEffect == null || m_ImpactEffectAnimator == null) return;
        m_ImpactEffect.SetActive(true);
        m_ImpactEffectAnimator.Play(0, 0, 0f);
        m_ImpactEffectAnimator.Update(0f);
    }
}
