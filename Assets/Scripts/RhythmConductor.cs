/*
    Author: Jose Escobedo
    Created on: Friday, September 12, 2025 14:43:13 EDT for UNITY ENGINE 6000.1.9f1
    Last modified: 2025-09-12 15:34:08 EDT

    Description:
    Main global script for the rhythm system of the game, named the conductor.
*/

using UnityEngine;

public class RhythmConductor : MonoBehaviour
{
    /*
        DESCRIPTION: RhythmConductor
        AUTHOR: Jose Escobedo

        The rhythm conductor keeps track of the current timing between player inputs and the music rhythm. Theoretically,
        outside code checks if the beat was hit on time or not and responds accordingly. Of course, the conductor needs
        to know what the current playing song is in order to figure important information such as tempo, current state,
        playback position, etc. Granted, some prudent information is not present on streamed music so we will have to
        make due with what we have.

        Anyways, why don't we talk about how this subsystem works?

        Every update, certain timer variables increment based on elapsed time. If outside code wants to know what's
        going on, like say to perform something awesome when a player attacks, the code calculates whether or not
        the button input was close enough to the beat and returns this information to outside code. For testing
        and possibly for deployment in real code(?), we also play a sound effect for player feedback.
    */

    // ----- Variables -----//

    // Currently playing background music.
    private AudioClip m_CurrentBGM;

    // Audio source for background music.
    [SerializeField] private AudioSource m_BgmSource;

    // Amount of time in seconds in which beats will register as a hit.
    [SerializeField] private float m_HitTimeThreshold;

    // Current time in seconds to calculate the beat.
    private float m_CurrentTime = 0.0f;

    // Beats per Minute for the current track. 120 BPM is one of the more popular tempos.
    private int m_Tempo = 120;

    // Pulses per Quarter Note (beat) for the current track. 100 PPQN is a value chosen randomly, but any number can work.
    private int m_PPQN = 100;

    // Time stretch percentage to modify the tempo on.
    private float m_TempoRate = 1.0f;

    // ----- Methods -----//

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Global object so cannot be killed. Called twice to make sure this object never gets unloaded.
        DontDestroyOnLoad(transform.gameObject);
        DontDestroyOnLoad(transform.root.gameObject);
        ResetMusic(120, 100);
        ResetCounters();
    }

    // Update is called once per frame
    void Update()
    {
        const int seconds_per_minute = 60;
        const float time_between_beats = seconds_per_minute / (float)m_Tempo * m_TempoRate;
        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime > time_between_beats)
        {
            m_CurrentTime -= time_between_beats;
        }
    }

    // Change the current BGM.
    public void ChangeBGM(AudioClip bgm_music, int tempo, int ppqn = 100)
    {
        m_CurrentBGM = bgm_music;
        ResetMusic(tempo, ppqn);
    }

    // Change the current tempo rate, which in turn non-destructively modifies the music tempo. Useful for speed effects!
    public void ChangeTempoRate(float tempo_rate)
    {
        m_TempoRate = tempo_rate;
    }

    // Returns true or false whether or not this function was called on beat.
    // This function can be used to tell outside code that something interesting should happen based on rhythm.
    public bool IsOnBeat()
    {
        // Explanation behind this code found in the comments in the class definition.
        const int seconds_per_minute = 60;
        const float time_between_beats = seconds_per_minute / (float)m_Tempo * m_TempoRate;
        return Math.Abs(time_between_beats - m_CurrentTime) < m_HitTimeThreshold;
    }

    // Reset music variables in case of music change.
    private void ResetMusic(int tempo, int ppqn)
    {
        m_CurrentTime = 0.0f;
        m_Tempo = tempo;
        m_PPQN = ppqn;
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
