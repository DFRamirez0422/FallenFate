using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    public class MusicAutoStarter : MonoBehaviour
    {
        // TODO: TESTING ONLY - Please remove when finished.
        [Tooltip("Name of the song for testing purposes - TESTING ONLY")]
        [SerializeField] private string m_TestSong;
        
        [SerializeField] private RhythmMusicPlayer music;
        
        void Start()
        {
            if (!music) music = GetComponent<RhythmMusicPlayer>();
            if (music != null)
            {
                // TODO: make the string something dynamic and an actual variable.
                music.StartSong(m_TestSong, 0.1); // Schedule start 0.1s in the future
                Debug.Log("[MusicAutoStarter] Music '" + m_TestSong + "' scheduled to start");
            }
        }
    }
}