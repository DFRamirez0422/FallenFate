using UnityEngine;
using UnityEngine.Rendering;
using NPA_Health_Components;

public class Player_Respawn : MonoBehaviour
{
    [SerializeField] private GameObject CurrentRespawnPoint;
    public GameObject CurrentCheckPoint;
    [SerializeField] private GameObject Player;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
            CurrentRespawnPoint = CurrentCheckPoint;
    }

}
