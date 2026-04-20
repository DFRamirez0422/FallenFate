using UnityEngine;
using Dypsloom.RhythmTimeline.Core.Managers;
using Dypsloom.RhythmTimeline.Effects;

public class Rythm_Test : MonoBehaviour
{   
    public void rythm_testPlayer_track(string trackName)
    {
        Debug.Log($"Player expected input processed on track {trackName}", gameObject);
    }
    

    public void rythm_testEnemy_track(string trackName)
    {
        Debug.Log($"Enemy expected input processed on track {trackName}", gameObject);
    }
}
