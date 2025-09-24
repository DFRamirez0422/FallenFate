/*
    Author: Jose Escobedo
    Created on: Friday, September 22, 2025 13:26 for UNITY ENGINE 6000.1.9f1

    Description:
    UI behavior script for the Music Bar to visualize the rhythm combat to the player.
*/

using System;
using System.Collections;
using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    /*
        DESCRIPTION: MusicBarUI
        AUTHOR: Jose Escobedo

        Code modified from the original PulseToBeat.cs file created by:
            MIKE HERNANDEZ

        Probably will drastically change once we get into talks with the UI designers as to
        what they want to see out of the music bar as well as what we would like to incorporate
        from Ratatan. Prototyping!

        Anyways...

        This script performs movement and updates of the music bar UI elements. If we were to base
        the UI on Ratatan, we would need a reference to all beat markers. To keep things simple for now,
        we just need a reference to the cursor. To start out this such simple task, the cursor will
        just teleport once it reaches the end. Rather gnarly, perhaps, but we will fix it in accordance
        to the game vision.

        TODO: talk with the UI designers and play the Ratatan demo to find out how to implement this behavior.
        TODO: fix up the code proper instead of a shoddy copy of PulseToBeat.cs
    */
    public class MusicBarUI : MonoBehaviour
    {
        [SerializeField] private RhythmMusicPlayer music;  // Music clock

        [Tooltip("Game object of the music cursor in the music bar.")]
        [SerializeField] private GameObject m_CursorObject;

        [Tooltip("Game object of the music staff itself.")]
        [SerializeField] private RectTransform m_StaffObject;

        [Header("Pulses")]

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
        
        private Vector3 startPosition; // Original position of UI
        private Vector3 startSize; // Original scale of UI
        
        private void Start()
        {
            if (!m_CursorObject)
            {
                throw new Exception("No music bar cursor objet was set. Aborting!");
            }

            if (!music) music = FindAnyObjectByType<RhythmMusicPlayer>();
            startSize = m_CursorObject.transform.localScale;
            startPosition = m_CursorObject.transform.position;

            // Moves the cursor back just enough to adjust for any lag from the song.
            // Probably should find a way to bring in the game's start time as well; a lot of
            // the lag I am encountering stems from just Unity itself.
            float lag_time = (float)(music.SongStartDSP % music.BeatSec);
            m_CursorObject.transform.Translate(-lag_time, 0.0f, 0.0f);
            
            if (autoPlay && music != null) 
                StartCoroutine(PulseRoutine());
        }

        // Smoothly return scale back to start size
        private void Update()
        {
            // Implement the movement speed based on the beats per second and the staff dimensions.
            // If the cursor moves past the end, snap it back to the beginning.
            // Of course this carries the assumption that the cursor is on the correct position to
            // begin with, so not very ideal.
            int num_beats_per_measure = 4;
            float staff_dimension = m_StaffObject.rect.width;
            float delta_speed = (staff_dimension / (float)num_beats_per_measure) / music.BeatSec * Time.deltaTime;
            m_CursorObject.transform.Translate(delta_speed, 0.0f, 0.0f);

            if (m_CursorObject.transform.position.x > startPosition.x + staff_dimension)
            {
                m_CursorObject.transform.position = startPosition;
            }

            m_CursorObject.transform.localScale = Vector3.Lerp(m_CursorObject.transform.localScale, startSize, Time.deltaTime * returnSpeed);
        }

        // Trigger a visual pulse
        public void Pulse()
        {
            m_CursorObject.transform.localScale = startSize * pulseSize;
        }

        // Calculates the beats per second given a dynamically-changing tempo.
        private double CalcBeatPerSec()
        {
            // Jose E.:
            // Recalculate if the tempo has been changed.
            // Clamping is needed to avoid locking Unity.
            // Removing the clamping will lock Unity entirely and require force close. Please do not remove.
            // Please do not ask what the magic numbers mean either. They're supposed to be sensible limits.
            double beatSec = 60 / (music.BPM * subdivision);
            return System.Math.Clamp(beatSec, 1.0 / 10.0, 2.0);
        }

        // Pulses every quarter note using the song's BPM
        IEnumerator PulseRoutine()
        {
            if (music == null || music.BPM <= 0) yield break;
            
            double beatSec = CalcBeatPerSec();
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
                    
                beatSec = CalcBeatPerSec();
            }
        }
    }
}