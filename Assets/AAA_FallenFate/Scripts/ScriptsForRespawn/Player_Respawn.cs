using UnityEngine;
using UnityEngine.Rendering;

public class Player_Respawn : MonoBehaviour
{
    public Transform CurrentRespawnPoint;
    public Transform CurrentCheckPoint = null;

    private void Update()
    {
        if (CurrentCheckPoint != null)
        CurrentRespawnPoint = CurrentCheckPoint;
    }

}
