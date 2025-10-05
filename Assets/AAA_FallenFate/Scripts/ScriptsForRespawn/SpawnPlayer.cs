using UnityEngine;
using NPA_Health_Components;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    public GameObject playerclone;
    [SerializeField] private GameObject SpawnPoint;
    

    private void Start()
    {
        playerclone = Instantiate(Player, SpawnPoint.transform.position, Quaternion.identity);
    }
}
