using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    public class MusicAutoStarter : MonoBehaviour
    {
        [SerializeField] private RhythmMusicPlayer music;
        [Tooltip("Startup delay in seconds")]
        [SerializeField] private float startDelay = 0.3f;
        
        void Start()
        {
            if (!music) music = GetComponent<RhythmMusicPlayer>();
            if (music != null)
            {
                music.StartSong(startDelay); 
                Debug.Log("[MusicAutoStarter] Music scheduled to start");
            }
        }
    }
}