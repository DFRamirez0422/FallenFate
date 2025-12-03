using UnityEngine;

public class AttackPitchSync : MonoBehaviour
{
    [Header("References")]
    public AudioProcessor audioProcessor;   // Reference to your music analyzer
    public AudioSource[] attackAudioSources; // Array of attack audio sources   

    [Header("Pitch Settings")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.4f;
    public float sensitivity = 4f; // Higher = more reactive

    private float currentMusicLevel = 0f;

    public int countAttack = 0;

    void Start()
    {
        if (audioProcessor != null)
        {
            // Subscribe to spectrum event
            audioProcessor.onSpectrum.AddListener(OnSpectrumUpdated);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayAttackSound();
        }

        if (countAttack < 0)
        {
            countAttack = attackAudioSources.Length - 1;
        }
    }

    private void OnSpectrumUpdated(float[] spectrum)
    {
        // Example metric: average of top 4 frequency bands
        float intensity =
            (spectrum[8] + spectrum[9] + spectrum[10] + spectrum[11]) * 0.25f;

        // Smooth and scale it
        currentMusicLevel = Mathf.Lerp(currentMusicLevel, intensity * sensitivity, 0.2f);
    }

    /// <summary>
    /// Call this when the player performs an attack
    /// </summary>
    public void PlayAttackSound()
    {
        float pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Clamp01(currentMusicLevel));
        attackAudioSources[countAttack].pitch = pitch;

        attackAudioSources[countAttack].Play();

        countAttack++;
        if (countAttack >= attackAudioSources.Length)
        {
            countAttack = 0;
        }

        if(attackAudioSources[countAttack-2].isPlaying)
        {
            attackAudioSources[countAttack-2].Stop();
        }

    }
}
