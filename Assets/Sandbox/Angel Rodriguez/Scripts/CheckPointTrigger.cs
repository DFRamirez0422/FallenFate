using UnityEngine;
using NPA_Health_Components;

public class CheckPointTrigger : MonoBehaviour
{
    Player_Respawn RespawnManager;
    SphereCollider SphereC;


    private void Awake()
    {
        RespawnManager = GameObject.FindGameObjectWithTag("RespawnManager").GetComponent<Player_Respawn>();
        SphereC = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RespawnManager.CurrentCheckPoint = this.gameObject;
            if(SphereC != null)
                SphereC.enabled = false;
        }
    }
}
