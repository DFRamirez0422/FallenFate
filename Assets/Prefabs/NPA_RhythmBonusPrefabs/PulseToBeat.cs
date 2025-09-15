using UnityEngine;
using System.Collections;

namespace Player.RhythmBonusPrefabs
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
            double nextBeat = music.SongStartDSP;
            
            while (AudioSettings.dspTime < nextBeat)
                yield return null;

            while (true)
            {
                Pulse();
                nextBeat += beatSec;
                
                // Wait until DSP time reaches next beat
                while (AudioSettings.dspTime < nextBeat)
                    yield return null;
            }
        }
    }
}