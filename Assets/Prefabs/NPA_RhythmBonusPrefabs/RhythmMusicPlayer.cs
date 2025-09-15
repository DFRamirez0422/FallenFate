using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    /// <summary>
    ///  Plays music locked to DSP time
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MusicDictionary))]
    public class RhythmMusicPlayer : MonoBehaviour
    {
        // TODO: Note by Jose E.
        // Delete these variables to make way for a modular approach to specifying songs instead.
        [Header("Audio")]
        [SerializeField] private AudioSource source; // Reference to AudioSource
        [SerializeField] private AudioClip clip;     // The song to play
        [SerializeField] private bool loop = true;   // Use Unity's built-in looping
        
        // TODO: Note by Jose E.
        // Delete this variable to make way for a modular approach to specifying songs instead.
        [Header("Tempo")]
        [Tooltip("Quarter-note BPM of this track")]
        [SerializeField] private float bpm = 120f; // Beats per minute
        
        // Public values read by RhythmBonusJudge
        public double SongStartDSP { get; private set; } = 0.0;   // DSP time when beat 0 began
        public float BPM          => bpm;                         // Return BPM
        public float BeatSec      => (bpm > 0f) ? 60f / bpm : 0f; // Seconds per quarter note

        private bool playing = false; // Is music currently playing?

        // This variable added by Jose E.
        // Reference to the script to retrieve songs by their names.
        private MusicDictionary m_MusicDictionary;
        
        void Reset()
        {
            source = GetComponent<AudioSource>(); // Auto-assign source if on same object
        }
        
        void Awake()
        {
            if (!m_MusicDictionary) m_MusicDictionary = GetComponent<MusicDictionary>();
            if (!source) source = GetComponent<AudioSource>(); // Ensure source exists
            if (clip) source.clip = clip;                      // Load song if set 
        }
        
        // Start playback locked to DSP clock
        //
        // Changed by Jose E.
        // - Added argument for specifying the song name to be looked up by the music player.
        public void StartSong(string song_name, double delaySec = 0.1)
        {
            // Bottom lines added by Jose E.
            var song_entry = m_MusicDictionary.GetMusicByName(song_name);
            if (song_entry is null) return;

            // Change tempo related variables.
            bpm = song_entry.Value.m_Tempo;

            // TODO: remove this line.
            if (!source || !clip) return;
            
            source.clip = song_entry.Value.m_MusicClip; // Assign clip to AudioSource
            source.playOnAwake = false;                 // Don't autoplay
            source.loop = loop;                         // Enable Unity's built-in loop
            
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