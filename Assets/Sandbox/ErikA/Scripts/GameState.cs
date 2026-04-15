using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    /// <summary>When true, <see cref="PlayerMovement"/> skips input and movement.</summary>
    /// <remarks>
    /// Static so it persists — if the scene unloads mid-camera-waypoint (e.g. LoadScene),
    /// the coroutine never clears it. We reset on scene load and in <see cref="CameraWaypointSystem"/> OnDestroy.
    /// </remarks>
    public static bool GameplayLocked;

    private void Awake()
    {
        GameplayLocked = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameplayLocked = false;
    }
}
