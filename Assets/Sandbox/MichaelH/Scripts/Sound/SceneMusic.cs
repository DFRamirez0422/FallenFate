using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private SoundDefinition music;

    private void Start()
    {
        if (music != null)
            MusicManager.instance.PlayMusic(music);
    }
}