using UnityEditor.Rendering;
using UnityEngine;
using NPA_Health_Components;

public class CheckPointTrigger : MonoBehaviour
{
    Player_Respawn PlayerCheck;
    SphereCollider SphereC;


    private void Awake()
    {
        PlayerCheck = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Player_Respawn>();
        SphereC = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCheck.CurrentCheckPoint = this.gameObject.transform;
            if(SphereC != null)
                SphereC.enabled = false;
        }
    }
}
