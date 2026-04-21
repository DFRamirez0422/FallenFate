using UnityEngine;
using UnityEngine.Video;
public class VideoPlayerAssignCam : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    void Awake()
    {
        videoPlayer.targetCamera = Camera.main.GetComponent<Camera>();
    }
}
