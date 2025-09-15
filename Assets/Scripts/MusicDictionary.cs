/*
    Author: Jose Escobedo
    Created on: Friday, September 14, 2025 12:42 EDT for UNITY ENGINE 6000.1.9f1

    Description:
    Script to convert between user-friendly names and the low-level information concerning filepaths and rhythm information.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicDictionary : MonoBehaviour
{
    /*
        DESCRIPTION: MusicDictionary
        AUTHOR: Jose Escobedo

        This class performs the retrieval and storage between the user-friednly names from our designers and programmers to a struct containing
        information about the song itself. This helps decoupling between a song and its filepath, tempo, and other key aspects, thus making
        the music system more modular without different places in the codebase separately keeping track of information.

        TODO: Will the game have dynamic tempos in their songs? That will require an overhaul of this system requiring a sequence of tempo
        change events to tell the music player.
    */
    [Serializable]
    public struct MusicEntry
    {
        [Tooltip("User-friendly name of this song.")]
        [SerializeField] internal string m_Name;

        [Tooltip("Resource to the song clip represented by this entry.")]
        [SerializeField] public AudioClip m_MusicClip;

        // Sidenote: This is a singular value for now. If the game has songs with changing tempos, this won't
        // work out too well. The tempo rate percentage can only do so much.
        [Tooltip("This song's base tempo in beats per minute.")]
        [SerializeField] public float m_Tempo;
    }

    // ----- Variables -----//

    [Tooltip("List of all songs that will be used with the rhythm system. The key represents the user-friendly name.")]
    [SerializeField] private MusicEntry[] m_MusicEntries;

    // Unity has no support for dictionaries but we do need the fast lookup time.
    private Dictionary<string, MusicEntry> m_BgmDictionary;

    // ----- Methods -----//

    void Start()
    {  
        m_BgmDictionary = new Dictionary<string, MusicEntry>();

        // Populate the dictionary with our entries.
        foreach (MusicEntry entry in m_MusicEntries)
        {
            m_BgmDictionary.Add(entry.m_Name, entry);
        }
    }

    // Retrieves the music entry from the list of all playable songs by the user-friendly name.
    // This string defines the name to be used by the entire codebase to identify songs without
    // needing to worry about keeping track of specific filenames, resources, or key information of the song.
    //
    // If the song doesn't exist, null will be returned.
    public MusicEntry? GetMusicByName(string name)
    {
        try
        {
            Debug.Log("Now loading the song '" + name + "'...");
            return m_BgmDictionary[name];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("The song '" + name + "' was not found! I cannot play!");
            return null;
        }
    }
}
