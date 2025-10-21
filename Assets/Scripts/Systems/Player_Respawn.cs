using UnityEngine;
using UnityEngine.Rendering;
using NPA_Health_Components;

public class Player_Respawn : MonoBehaviour
{
    [SerializeField] private GameObject CurrentRespawnPoint;
    public GameObject CurrentCheckPoint;

    [SerializeField] private bool IsDead;
    [SerializeField] private GameObject Player;

    private void Awake()
    {
        IsDead = false;
        SpawnPlayer PLAYER = GameObject.FindGameObjectWithTag("StartingSpawnPoint").GetComponent<SpawnPlayer>();
        Player = PLAYER.playerclone;
    }
    // Update is called once per frame
    void Update()
    {
            CurrentRespawnPoint = CurrentCheckPoint;

        if (IsDead)
        {
            Player.transform.position = CurrentRespawnPoint.transform.position;
            Health Health = Player.gameObject.GetComponent<Health>();
        }
    }

    public void DieAndRespawn(bool Dead)
    {
        Debug.Log($"{gameObject.name} has died!");
        IsDead = Dead;
    }

}
