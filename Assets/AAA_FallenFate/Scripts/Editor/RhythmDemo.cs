/*
    Author: Jose Escobedo
    Created on: Friday, September 12, 2025 15:34:08 EDT for UNITY ENGINE 6000.1.9f1
    Last modified: 2025-09-12 15:34:08 EDT

    Description:
    Demo script for testing the rhythm subsystem.
*/

using UnityEngine;

public class RhythmDemo : MonoBehaviour
{
    /*
        --------------------------------------------------
        U S E D   F O R   T E S T I N G
        --------------------------------------------------

        USED FOR TESTING THE SUBSYSTEM

        DESCRIPTION: RhythmConductor
        AUTHOR: Jose Escobedo

        This script is used to test the rhythm subsystem in isolation. Please do not use in production code as is.
    */

    // ----- Variables -----//

    // Game script for the rhythm conductor.
    [SerializeField] private RhythmConductor m_Conductor;

    // Currently playing background music.
    [SerializeField] private AudioClip m_CurrentBGM;

    // UI text field that counts the current streak.
    [SerializeField] private TMPro.TextMeshProUGUI m_TempoUI;

    // UI text field that counts the number of hits.
    [SerializeField] private TMPro.TextMeshProUGUI m_NumHitsUICounter;

    // UI text field that counts the number of misses.
    [SerializeField] private TMPro.TextMeshProUGUI m_NumMissUICounter;

    // UI text field that counts the current streak.
    [SerializeField] private TMPro.TextMeshProUGUI m_StreakUICounter;

    // Audio source for sound effects.
    [SerializeField] private UnityEngine.AudioSource m_SeSource;

    // Audio clip for a feedback sound effect to play for a beat hit.
    [SerializeField] private UnityEngine.AudioClip m_BeatHitSe;

    // Audio clip for a feedback sound effect to play for a beat miss.
    [SerializeField] private UnityEngine.AudioClip m_BeatMissSe;

    // Base tempo value in beats per minute.
    [SerializeField] private int m_BgmTempo = 120;

    // Base pulse-per-quarter-beat value.
    [SerializeField] private int m_BgmPulsePerBeat = 100;

    // Counts how many beats were hit in the current streak.
    private int m_HitStreak = 0;

    // Counts how many total beats were hit in rhythm.
    private int m_NumHits = 0;

    // Counts how many total beats were missed in rhythm.
    private int m_NumMisses = 0;

    // ----- Methods -----//

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetMusic(m_BgmTempo, m_BgmPulsePerBeat);
        ResetCounters();
    }

    // Update is called once per frame
    // Processes the player input and responds accordingly.
    void Update()
    {
        // Check periodically whether or not there is a beat to hit.
        if (m_Conductor.IsOnBeat())
        {
            // Any button or key will work but as long as it makes sense.
            if (UnityEngine.Input.GetButton("Jump"))
            {
                m_NumHits += 1;
                m_HitStreak += 1;
            }
            else
            {
                m_NumMisses += 1;
                m_HitStreak = 0;
            }
        }

        UpdateUI();
    }

    // USED FOR TESTING : Updates the UI to display information to the player concerning rhythm.
    private void UpdateUI()
    {
        m_TempoUI.text = m_BgmTempo.ToString();
        m_NumHitsUICounter.text = m_NumHits.ToString();
        m_NumMissUICounter.text = m_NumMisses.ToString();
        m_StreakUICounter.text = m_HitStreak.ToString();
    }

    // Reset music variables in case of music change.
    private void ResetMusic(int tempo, int ppqn)
    {
        m_Conductor.ChangeBGM(m_CurrentBGM, tempo, ppqn);
    }

    // Reset beat counter variables to default values.
    private void ResetCounters()
    {
        m_HitStreak = 0;
        m_NumHits = 0;
        m_NumMisses = 0;
    }

    // Reset beat streak variables.
    private void ResetStreak()
    {
        m_HitStreak = 0;
    }
}
