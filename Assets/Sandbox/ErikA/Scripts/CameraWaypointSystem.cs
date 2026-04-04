using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraWaypointSystem : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera cam;
    public Transform[] Waypoints;
    public Transform Player;

    [Header("Proxy Target (empty GameObject)")]
    public Transform camTarget;

    [Header("Timing")]
    public float moveDuration = 2f;
    public float pauseTime = 5f;

    private bool isPlaying;
    private Coroutine routine;
    
    LetterBoxController letterBox;
    
    PlayerMovement playerMovement;
    PlayerAnimator playerAnimator;

    void Awake()
    {
        
        letterBox = FindFirstObjectByType<LetterBoxController>();
    }
   
        
    
    public void StartWaypointSequence()
    {
        if (isPlaying) return;

        if (letterBox != null)
            letterBox.EnableBars();

        if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(WaypointRoutine());
    }

    private IEnumerator WaypointRoutine()
    {
        isPlaying = true;
        GameState.GameplayLocked = true;
        

        // Start proxy at current player position
        camTarget.position = Player.position;

        // Follow proxy during cinematic
        cam.Follow = camTarget;
        cam.LookAt = camTarget; 

        foreach (Transform point in Waypoints)
        {
            if (!point) continue;

            
            Vector3 dest = point.position;
            dest.z = camTarget.position.z;

            yield return MoveTarget(camTarget, dest, moveDuration);
            yield return new WaitForSeconds(pauseTime);
        }

        // Return to player smoothly
        Vector3 back = Player.position;
        back.z = camTarget.position.z;
        yield return MoveTarget(camTarget, back, moveDuration);

        // Restore gameplay follow
        cam.Follow = Player;
        cam.LookAt = Player;
        
        if (letterBox != null)
            letterBox.DisableBars();

        isPlaying = false;
        routine = null;
        GameState.GameplayLocked = false;
    }

    private IEnumerator MoveTarget(Transform t, Vector3 targetPos, float duration)
    {
        Vector3 startPos = t.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float a = Mathf.Clamp01(time / duration); //alpha
            
            t.position = Vector3.Lerp(startPos, targetPos, a);
            yield return null;
        }

        t.position = targetPos;
    }
}