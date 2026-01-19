using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Camera boundaries finder to find camera boundaries across scenes regardless of concrete shape being used.
/// </summary>
public class ConfinerFinder : MonoBehaviour
{
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
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        GameObject camera_bounds = GameObject.FindWithTag("CameraBounds");
        Collider2D collider = camera_bounds.GetComponent<PolygonCollider2D>();

        if (!collider)
        {
            collider = camera_bounds.GetComponent<BoxCollider2D>();
        }

        confiner.BoundingShape2D = collider;
    }
}
