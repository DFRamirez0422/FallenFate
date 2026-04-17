using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private SoundDefinition music;

    private void Start()
    {
        if (music == null) return;

        MusicManager.instance.PlayMusic(music);
    }
}