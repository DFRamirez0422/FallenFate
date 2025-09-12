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

    // ----- Variables -----//

    // Game script for the rhythm conductor.

    // Audio source for sound effects.
    [SerializeField] private AudioSource m_SeSource;

    // Audio clip for a feedback sound effect to play for a beat hit.
    [SerializeField] private AudioClip m_BeatHitSe;

    // Audio clip for a feedback sound effect to play for a beat miss.
    [SerializeField] private AudioClip m_BeatMissSe;

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

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    // Processes the player input and responds accordingly.
    private void ProcessInput()
    {
        ;
    }

    // USED FOR TESTING : Updates the UI to display information to the player concerning rhythm.
    private void UpdateUI()
    {
        ;
    }
}
