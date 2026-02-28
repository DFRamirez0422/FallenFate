using UnityEngine;

public class EnemyHitScript : MonoBehaviour
{
    [SerializeField] private AudioClip m_HitSound;
    [SerializeField] private AudioSource m_EnemySoundSource;

    // Impact effect object (can be a prefab or child object)
    [SerializeField] private GameObject m_ImpactEffect;

    // Animator is optional to assign manually — will be found automatically if missing
    [SerializeField] private Animator m_ImpactEffectAnimator;

    // Optional animator state name. Leave empty to use state index 0.
    [SerializeField] private string m_ImpactStateName = "";

    [SerializeField] private SpriteRenderer m_EnemySprite;
    [SerializeField] private Material m_FlashMaterial;
    [SerializeField] private float m_HitDuration = 0.2f;

    private Material m_OriginalMaterial;

    private void Awake()
    {
        // Auto-assign common components if not set in inspector
        if (m_EnemySprite == null)
            m_EnemySprite = GetComponent<SpriteRenderer>();

        if (m_EnemySoundSource == null)
            m_EnemySoundSource = GetComponent<AudioSource>();

        // Cache original material for flash reset
        if (m_EnemySprite != null)
            m_OriginalMaterial = m_EnemySprite.material;

        // Prepare impact effect
        if (m_ImpactEffect != null)
        {
            // Find animator even if it lives on a child object
            if (m_ImpactEffectAnimator == null)
                m_ImpactEffectAnimator =
                    m_ImpactEffect.GetComponentInChildren<Animator>(true);

            // Keep effect hidden until used
            m_ImpactEffect.SetActive(false);
        }
    }

    // Plays hit audio once
    public void PlayHitSound()
    {
        if (m_EnemySoundSource != null && m_HitSound != null)
            m_EnemySoundSource.PlayOneShot(m_HitSound);
    }

    // Temporarily swaps material for hit flash effect
    public void FlashWhite()
    {
        if (m_EnemySprite == null || m_FlashMaterial == null) return;

        CancelInvoke(nameof(ResetSpriteMaterial));
        m_EnemySprite.material = m_FlashMaterial;
        Invoke(nameof(ResetSpriteMaterial), m_HitDuration);
    }

    // Restores original sprite material
    private void ResetSpriteMaterial()
    {
        if (m_EnemySprite != null && m_OriginalMaterial != null)
            m_EnemySprite.material = m_OriginalMaterial;
    }

    // Activates and restarts the impact animation
    public void ImpactEffect()
    {
        if (m_ImpactEffect == null) return;

        // Safety check in case prefab changed at runtime
        if (m_ImpactEffectAnimator == null)
            m_ImpactEffectAnimator =
                m_ImpactEffect.GetComponentInChildren<Animator>(true);

        if (m_ImpactEffectAnimator == null) return;

        // Enable effect object
        m_ImpactEffect.SetActive(true);

        // Restart animation from frame 0
        if (!string.IsNullOrEmpty(m_ImpactStateName))
            m_ImpactEffectAnimator.Play(m_ImpactStateName, 0, 0f);
        else
            m_ImpactEffectAnimator.Play(0, 0, 0f);

        // Force animator to update immediately this frame
        m_ImpactEffectAnimator.Update(0f);
    }
}