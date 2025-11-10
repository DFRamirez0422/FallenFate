using UnityEngine;
using UnityEngine.Rendering;
using NPA_Health_Components;

public class Player_Respawn : MonoBehaviour
{
    [SerializeField] private GameObject CurrentRespawnPoint;
    public GameObject CurrentCheckPoint;

    // Update is called once per frame
    void Update()
    {
            CurrentRespawnPoint = CurrentCheckPoint;
    }

}
