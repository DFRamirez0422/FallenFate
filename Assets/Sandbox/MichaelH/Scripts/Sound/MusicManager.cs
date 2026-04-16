using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private SoundDefinition currentMusic;
    
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    [SerializeField] private bool shouldFade = true;

    /// <summary>
    /// Initializes the MusicManager singleton instance and sets up the active and inactive audio sources for crossfading.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            activeSource = musicSourceA;
            inactiveSource = musicSourceB;
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Plays the specified music with a crossfade. If the same music is already playing, it will not restart.
    /// </summary>
    public void PlayMusic(SoundDefinition music, float fadeTime = 1f)
    {
        if (music == null) return;
        
        if (!shouldFade) fadeTime = 0f;
        if (currentMusic == music && activeSource.isPlaying) return;
        AudioClip clip = music.GetClip();
        if (clip == null) return;

        currentMusic = music;

        inactiveSource.clip = clip;
        inactiveSource.volume = 0f;
        inactiveSource.pitch = music.GetPitch();
        inactiveSource.loop = true;

        if (music.mixerGroup != null)
            inactiveSource.outputAudioMixerGroup = music.mixerGroup;

        inactiveSource.Play();

        // prevent old crossfades from interfering
        StopAllCoroutines();

        StartCoroutine(Crossfade(fadeTime, music.GetVolume()));
    }
    
    /// <summary>
    /// Crossfades between the active and inactive music sources over the specified duration,
    /// fading out the active source and fading in the inactive source to the target volume.
    /// Once the crossfade is complete, the active source is stopped and the sources are swapped.
    /// </summary>
    private IEnumerator Crossfade(float duration, float targetVolume)
    {
        duration = Mathf.Max(0.01f, duration); // Clamp duration to avoid division by zero

        float time = 0f;
        float startVolume = activeSource.volume;

        // Crossfade loop: gradually decrease active source volume and increase inactive source volume
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        inactiveSource.volume = targetVolume;
        activeSource.volume = 0f;

        activeSource.Stop();

        // Swap sources
        (activeSource, inactiveSource) = (inactiveSource, activeSource);

        // SAFETY: ensure new active is actually playing
        if (!activeSource.isPlaying && activeSource.clip != null)
        {
            activeSource.Play();
        }
    }

    public void StopMusic(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutAll(fadeTime));
        currentMusic = null;
    }
    
    /// <summary>
    /// Fades out both music sources over the specified duration, then stops them.
    /// This is used when stopping music to ensure a smooth fade-out regardless of which source is active.
    /// </summary>
    private IEnumerator FadeOutAll(float duration)
    {
        duration = Mathf.Max(0.01f, duration); // Clamp duration to avoid division by zero

        float time = 0f;

        float startA = musicSourceA.volume;
        float startB = musicSourceB.volume;

        // Fade out loop: gradually decrease both sources' volumes to zero
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            musicSourceA.volume = Mathf.Lerp(startA, 0f, t);
            musicSourceB.volume = Mathf.Lerp(startB, 0f, t);

            yield return null;
        }

        musicSourceA.volume = 0f;
        musicSourceB.volume = 0f;

        musicSourceA.Stop();
        musicSourceB.Stop();
    }
    
    private void LateUpdate()
    {
        if (currentMusic == null) return;

        // If BOTH sources are stopped → recover
        if (!musicSourceA.isPlaying && !musicSourceB.isPlaying)
        {
            Debug.Log("Both music sources stopped. Recovering...");

            AudioClip clip = currentMusic.GetClip();
            if (clip == null) return;

            activeSource.clip = clip;
            activeSource.volume = currentMusic.GetVolume();
            activeSource.pitch = currentMusic.GetPitch();
            activeSource.loop = true;

            activeSource.Play();
        }
    }
}