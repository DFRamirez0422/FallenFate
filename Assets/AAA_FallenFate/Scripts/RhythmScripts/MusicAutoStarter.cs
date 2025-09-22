using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    public class MusicAutoStarter : MonoBehaviour
    {
        // TODO: TESTING ONLY - Please remove when finished.
        [Tooltip("Name of the song for testing purposes - TESTING ONLY")]
        [SerializeField] private string m_SongName;
        
        [SerializeField] private RhythmMusicPlayer music;
        [Tooltip("Startup delay in seconds")]
        [SerializeField] private float startDelay = 0.3f;
        
        void Start()
        {
            if (!music) music = GetComponent<RhythmMusicPlayer>();
            if (music != null)
            {
<<<<<<< HEAD:Assets/Prefabs/NPA_RhythmBonusPrefabs/MusicAutoStarter.cs
                // TODO: make the string something dynamic and an actual variable.
                music.StartSong(m_SongName, 0.1); // Schedule start 0.1s in the future
                Debug.Log("[MusicAutoStarter] Music '" + m_SongName + "' scheduled to start");
=======
                music.StartSong(startDelay); 
                Debug.Log("[MusicAutoStarter] Music scheduled to start");
>>>>>>> main:Assets/AAA_FallenFate/Scripts/RhythmScripts/MusicAutoStarter.cs
            }
        }
    }
}