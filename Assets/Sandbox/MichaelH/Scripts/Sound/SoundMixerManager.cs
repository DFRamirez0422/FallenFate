using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    // NOTES: Commented out lines are the old way of setting volume, which was linear.
    // The new way uses a logarithmic scale to convert the linear volume level to decibels
    
    [SerializeField] private AudioMixer audioMixer;
    
    /// <summary>
    /// Sets the master volume level in the audio mixer. The input level is expected to be in the range of 0.0001 to 1,
    /// where 0.0001 corresponds to -80 dB (effectively silent) and 1 corresponds to 0 dB (full volume).
    /// The method converts the linear volume level to decibels using a logarithmic scale before setting it in the audio mixer.
    /// </summary>
    /// <param name="level"></param>
    public void SetMasterVolume(float level)
    {
        // audioMixer.SetFloat("MasterVolume", level);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }
    
    public void SetSoundFXVolume(float level)
    {
        // audioMixer.SetFloat("SoundFXVolume", level);
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
    }
    
    public void SetMusicVolume(float level)
    {
        // audioMixer.SetFloat("MusicVolume", level);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }
}
