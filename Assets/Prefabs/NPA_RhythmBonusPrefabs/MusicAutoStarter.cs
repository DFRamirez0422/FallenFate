using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    public class MusicAutoStarter : MonoBehaviour
    {
        [SerializeField] private RhythmMusicPlayer music;
        
        void Start()
        {
            if (!music) music = GetComponent<RhythmMusicPlayer>();
            if (music != null)
            {
                // TODO: make the string something dynamic and an actual variable.
                music.StartSong("Catchy Test", 0.1); // Schedule start 0.1s in the future
                Debug.Log("[MusicAutoStarter] Music scheduled to start");
            }
        }
    }
}