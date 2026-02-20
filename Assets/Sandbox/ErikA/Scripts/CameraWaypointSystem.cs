using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
public class CameraWaypointSystem : MonoBehaviour
{
    public Transform[] Waypoints;
    public Transform Player;
    public CinemachineCamera camera;

    private bool isActive;
    
    public float pauseTime = 5f;
    public float blendTime = 2f;
              
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        if(!camera)  camera = GetComponent<CinemachineCamera>();

    }

    public void StartWaypointSequence()
    {
        Debug.Log("Starting Sequence");
        StartCoroutine(WaypointRoutine());
    }
    private IEnumerator WaypointRoutine()
    {
        isActive = true;
        //TODO - FREEZE PLAYER & ENEMY MOVEMENT
        
        foreach (Transform target in Waypoints)
        {
            camera.Follow = target;
            camera.LookAt = target;  
            yield return new WaitForSeconds(pauseTime);
        }
        camera.Follow = Player;
        camera.LookAt = Player;
        isActive = false;
        //TODO - UNFREEZE PLAYER & ENEMY MOVEMENT
    }
    
}
