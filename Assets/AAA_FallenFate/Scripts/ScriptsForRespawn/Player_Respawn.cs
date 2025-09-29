using UnityEngine;
using UnityEngine.Rendering;
using NPA_Health_Components;

public class Player_Respawn : MonoBehaviour
{
    [SerializeField] private GameObject CurrentRespawnPoint;
    public GameObject CurrentCheckPoint;
    public Transform playerRespawn;
    [SerializeField] private bool IsDead;

    private void Start()
    {
        playerRespawn = this.transform;
        CurrentRespawnPoint = CurrentCheckPoint;
        playerRespawn.position = CurrentRespawnPoint.transform.position;
        IsDead = false;
    }
    // Update is called once per frame
    void Update()
    {
        CurrentRespawnPoint = CurrentCheckPoint;
        if (IsDead)
        {
            this.transform.localPosition = playerRespawn.position;
            Health Health = this.gameObject.GetComponent<Health>();
        }
    }

    public void DieAndRespawn(bool Dead)
    {
        playerRespawn.position = CurrentRespawnPoint.transform.position;
        Debug.Log($"{gameObject.name} has died!");
        IsDead = Dead;
    }

}
