using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Sound Definition")]
public class SoundDefinition : ScriptableObject
{
    [Header("Clips (1 or many)")]
    public AudioClip[] clips;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 volumeRandomRange = new Vector2(1f, 1f); // multiplier range

    [Header("Pitch")]
    public Vector2 pitchRange = new Vector2(1f, 1f);

    [Header("Routing (optional)")]
    public AudioMixerGroup mixerGroup;
    
    private int lastIndex = -1; // store last index

    // Returns a random AudioClip from the clips array, or null if the array is empty. Stores last index to avoid immediate repeats when multiple clips are available.
    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0) return null;
        if (clips.Length == 1) return clips[0];

        int index = Random.Range(0, clips.Length);

        if (index == lastIndex)
            index = (index + 1) % clips.Length;

        lastIndex = index;
        return clips[index];
    }

    // Returns a volume value that is the base volume multiplied by a random factor within the specified range, clamped between 0 and 1.
    public float GetVolume()
    {
        float mult = Random.Range(volumeRandomRange.x, volumeRandomRange.y);
        return Mathf.Clamp01(volume * mult);
    }

    // Returns a random pitch value within the specified range.
    public float GetPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}