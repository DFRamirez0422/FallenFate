using Player.RhythmBonusPrefabs;
using System.Collections;
using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    public class PulseToBeat : MonoBehaviour
    {
        [SerializeField] private RhythmMusicPlayer music;  // Music clock
        [Tooltip("How big pulse gets")]
        [SerializeField] private float pulseSize = 1.15f;
        
        [Tooltip("How fast pulse returns")]
        [SerializeField] private float returnSpeed = 5f;
        
        [Tooltip("1=Quarter, 1=Eighths, 4=Sixteenths, 0.5=Half Notes")]
        [SerializeField] private float subdivision = 1f;   // Which note value to follow
        
        [Tooltip("Auto-pulse with beat?")]
        [SerializeField] private bool autoPlay = true;

        [Header("Calibration")]
        [Tooltip("Extra offset (in seconds) to shift pulses later/earlier")]
        [SerializeField] private float pulseOffsetSec = 0f;
        
        private Vector3 startSize; // Original scale of UI
        
        private void Start()
        {
            if (!music) music = FindAnyObjectByType<RhythmMusicPlayer>();
            startSize = transform.localScale;
            
            if (autoPlay && music != null) 
                StartCoroutine(PulseRoutine());
        }

        // Smoothly return scale back to start size
        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
        }

        // Trigger a visual pulse
        public void Pulse()
        {
            transform.localScale = startSize * pulseSize;
        }

        // Pulses every quarter note using the song's BPM
        IEnumerator PulseRoutine()
        {
            if (music == null || music.BPM <= 0) yield break;
            
            double beatSec = 60 / (music.BPM * subdivision);
            double nextBeat = music.SongStartDSP + pulseOffsetSec + beatSec;
            
            while (AudioSettings.dspTime < nextBeat)
                yield return null;

            while (true)
            {
                Pulse();
                nextBeat += beatSec;

                // Wait until DSP time reaches next beat
                while (AudioSettings.dspTime < nextBeat)
                    yield return null;

                // Jose E.:
                // Recalculate if the tempo has been changed.
                // Clamping is needed to avoid locking Unity.
                // Removing the clamping will lock Unity entirely and require force close. Please do not remove.
                // Please do not ask what the magic numbers mean either. They're supposed to be sensible limits.
                beatSec = 60 / (music.BPM * subdivision);
                beatSec = System.Math.Clamp(beatSec, 1.0 / 10.0, 1.0);
            }
        }
    }
}