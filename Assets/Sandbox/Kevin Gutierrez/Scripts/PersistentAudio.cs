using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource source;      // assign in Inspector
    public AudioClip[] clips;       // assign SFX clips in Inspector

    Dictionary<string, AudioClip> clipMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        clipMap = new Dictionary<string, AudioClip>();
        foreach (var c in clips)
            if (c != null && !clipMap.ContainsKey(c.name))
                clipMap[c.name] = c;
    }

    public static void Play(string clipName, float volume = 1f)
    {
        if (Instance == null) return;
        if (!Instance.clipMap.TryGetValue(clipName, out var clip)) return;
        if (Instance.source == null) return; // safe-guard
        Instance.source.PlayOneShot(clip, volume);
    }
}
