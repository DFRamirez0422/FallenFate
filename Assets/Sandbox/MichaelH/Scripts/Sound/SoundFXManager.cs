using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Play(SoundDefinition sfx, Transform spawnTransform)
    {
        if (sfx == null || spawnTransform == null) return;
        
        // Debug.LogError($"[SFX] Playing '{sfx.name}' at '{spawnTransform.root.name}'");

        AudioClip clip = sfx.GetClip();
        if (clip == null) return;

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;
        audioSource.volume = sfx.GetVolume();
        audioSource.pitch = sfx.GetPitch();

        if (sfx.mixerGroup != null)
            audioSource.outputAudioMixerGroup = sfx.mixerGroup;

        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length / Mathf.Max(0.01f, audioSource.pitch));
    }
}