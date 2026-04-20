using UnityEngine;

public class Auto_Full_health : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.GetComponent<PlayerHealth>().ChangeHealth(4);
        
    }
}
