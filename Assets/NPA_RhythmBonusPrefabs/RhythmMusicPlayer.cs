using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    /// <summary>
    ///  Plays music locked to DSP time
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RhythmMusicPlayer : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource source; // Reference to AudioSource
        [SerializeField] private AudioClip clip;     // The song to play
        [SerializeField] private bool loop = true;   // Use Unity's built-in looping
        
        [Header("Tempo")]
        [Tooltip("Quarter-note BPM of this track")]
        [SerializeField] private float bpm = 120f; // Beats per minute
        
        // Public values read by RhythmBonusJudge
        public double SongStartDSP { get; private set; } = 0.0;   // DSP time when beat 0 began
        public float BPM          => bpm;                         // Return BPM
        public float BeatSec      => (bpm > 0f) ? 60f / bpm : 0f; // Seconds per quarter note
        
        private bool playing = false; // Is music currently playing?
        
        void Reset()
        {
            source = GetComponent<AudioSource>(); // Auto-assign source if on same object
        }
        
        void Awake()
        {
            if (!source) source = GetComponent<AudioSource>(); // Ensure source exists
            if (clip) source.clip = clip;                      // Load song if set 
        }
        
        // Start playback locked to DSP clock
        public void StartSong(double delaySec = 0.1)
        {
            if (!source || !clip) return;
            
            source.clip = clip;             // Assign clip to AudioSource
            source.playOnAwake = false;     // Don't autoplay
            source.loop = loop;             // Enable Unity's built-in loop
            
            double dspNow = AudioSettings.dspTime;                        // Current global audio clock
            double dspStart = dspNow + Mathf.Max(0.02f, (float)delaySec); // Schedule slightly ahead
            SongStartDSP = dspStart;                                      // Record when beat 0 starts

            source.time = 0f;               // Start at beginning of clip
            source.PlayScheduled(dspStart); // Schedule start on DSP clock
            playing = true;                 // Mark as playing
        }
        
        // Stop playback
        public void StopSong()
        {
            playing = false;           // Reset playing state
            if (source) source.Stop(); // Stop the AudioSource
        }
        
        // Seconds elapsed since beat zero
        public double GetElapsedSec()
        {
            if (!playing) return 0.0;                    // Return 0 if not playing
            return AudioSettings.dspTime - SongStartDSP; // DSP time offset by start
        }
        
        // Beats elapsed since beat zero
        public double GetElapsedBeats()
        {
            return (BeatSec > 0f) ? GetElapsedSec() / BeatSec : 0.0;
        }
    }
}