using UnityEditor.Rendering;
using UnityEngine;
using NPA_Health_Components;

public class CheckPointTrigger : MonoBehaviour
{
    Player_Respawn Player;
    SphereCollider SphereC;


    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Respawn>();
        SphereC = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.CurrentCheckPoint = this.gameObject;
            if(SphereC != null)
                SphereC.enabled = false;
        }
    }
}
