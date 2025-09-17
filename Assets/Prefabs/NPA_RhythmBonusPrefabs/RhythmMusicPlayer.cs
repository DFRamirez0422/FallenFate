using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    /// <summary>
    ///  Plays music locked to DSP time
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MusicDictionary))]
    public class RhythmMusicPlayer : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource source; // Reference to AudioSource
        [SerializeField] private MusicDictionary m_MusicDictionary; // Reference to the script to retrieve songs by their names.
        
        [Tooltip("Song speed as a multiplier relative to the base tempo.")]
        [SerializeField] private float m_TempoRate = 1.0f; // change the rhythm tempo without affecting the base value
        
        [Tooltip("Whether or not to loop the song once it reaches the end.")]
        [SerializeField] private bool loop = true;   // Use Unity's built-in looping
        
        private float bpm = 120f; // Beats per minute used by the code
        private bool m_IsPlaying = false; // Boolean flag to determine whether or not a song is playing.
        
        // Public values read by RhythmBonusJudge
        public double SongStartDSP { get; private set; } = 0.0;                 // DSP time when beat 0 began
        public float BPM          => bpm * m_TempoRate;                         // Return BPM after modifying
        public float BeatSec      => (bpm > 0f) ? 60f / bpm * m_TempoRate : 0f; // Seconds per quarter note
        public bool IsPlaying     => m_IsPlaying;                               // Whether or not a song is playing.

        private bool playing = false; // Is music currently playing?
        
        void Reset()
        {
            source = GetComponent<AudioSource>(); // Auto-assign source if on same object
        }
        
        void Awake()
        {
            if (!m_MusicDictionary) m_MusicDictionary = GetComponent<MusicDictionary>();
            if (!source) source = GetComponent<AudioSource>(); // Ensure source exists
        }
        
        // Start playback locked to DSP clock
        //
        // Changed by Jose E.
        // - Added argument for specifying the song name to be looked up by the music player.
        public void StartSong(string song_name, double delaySec = 0.1)
        {
            // Jose E.
            // Retrieve the song by its name and check if it exists. If it doesn't,
            // return as there's nothing else to be done.
            var song_entry = m_MusicDictionary.GetMusicByName(song_name);
            m_IsPlaying = (song_entry is not null);

            if (song_entry is null) return;

            // Change tempo related variables.
            // TODO: how to make tempo rate affect the tempo mid-song? Is this even needed?
            bpm = song_entry.Value.m_Tempo * m_TempoRate;

            if (!source) return;
            
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