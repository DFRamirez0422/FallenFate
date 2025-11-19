using UnityEngine;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    // Singleton instance for global access
    public static SoundFXManager Instance { get; private set; }

    [Header("Audio Source Setup")]
    [SerializeField] private AudioSource source; // Main audio source for playing sound effects

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] clips; // All sound effects registered in the manager

    private Dictionary<string, AudioClip> clipMap; // Fast lookup by clip name

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure there’s always an AudioSource
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        // Register all clips by name
        clipMap = new Dictionary<string, AudioClip>();
        foreach (var c in clips)
        {
            if (c != null)
                clipMap.TryAdd(c.name, c);
        }
    }
    
    // Plays a specific clip by name
    public static void Play(string clipName, float volume = 1f, float pitchVariation = 0f)
    {
        if (Instance == null || Instance.source == null) return;

        if (!Instance.clipMap.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"[SoundFXManager] Clip '{clipName}' not found!");
            return;
        }

        // Adds slight random pitch variation for natural sound
        Instance.source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        Instance.source.PlayOneShot(clip, volume);
    }
    
    // Plays a random clip from a list of names
    public static void PlayRandom(string[] clipNames, float volume = 1f, float pitchVariation = 0.1f)
    {
        if (clipNames == null || clipNames.Length == 0) return;
        var name = clipNames[Random.Range(0, clipNames.Length)];
        Play(name, volume, pitchVariation);
    }
    
    // Stops any currently playing sound
    public static void Stop()
    {
        if (Instance == null || Instance.source == null) return;
        Instance.source.Stop();
    }
    
    // Plays a clip in 3D space at a world position
    public static void PlayAtPosition(string clipName, Vector3 position, float volume = 1f)
    {
        if (Instance == null || !Instance.clipMap.TryGetValue(clipName, out var clip)) return;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}