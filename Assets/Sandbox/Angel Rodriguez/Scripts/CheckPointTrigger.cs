using UnityEditor.Rendering;
using UnityEngine;
using NPA_Health_Components;

public class CheckPointTrigger : MonoBehaviour
{
    private Player_Respawn _CheckPoint;
    private SphereCollider SphereC;


    private void Awake()
    {
        _CheckPoint = GameObject.FindGameObjectWithTag("RespawnManager").GetComponent<Player_Respawn>();

        SphereC = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _CheckPoint.CurrentCheckPoint = this.gameObject;

            if(SphereC != null)
                SphereC.enabled = false;
        }
    }
}
