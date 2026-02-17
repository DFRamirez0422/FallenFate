using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Camera manager script to automatically manage particular settings for the Cinemachine engine.
    /// 
    /// To start with, this script will automaticlly locate the player object when the scene is loaded.
    /// </summary>

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CinemachineCamera camera = GetComponent<CinemachineCamera>();
        camera.Target.TrackingTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
