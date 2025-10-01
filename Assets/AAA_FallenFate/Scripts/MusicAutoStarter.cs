using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    public class MusicAutoStarter : MonoBehaviour
    {
        // TODO: CURRENTLY TESTING ONLY - make the string something dynamic for production code.
        [Tooltip("Name of the song for testing purposes - TESTING ONLY")]
        [SerializeField] private string m_SongName;
        
        [SerializeField] private RhythmMusicPlayer music;
        [Tooltip("Startup delay in seconds")]
        [SerializeField] private float startDelay = 0.1f;
        
        void Start()
        {
            if (!music) music = GetComponent<RhythmMusicPlayer>();
            if (music != null)
            {
                music.StartSong(m_SongName, startDelay); // Schedule start 0.1s in the future
                Debug.Log("[MusicAutoStarter] Music '" + m_SongName + "' scheduled to start");
            }
        }
    }
}