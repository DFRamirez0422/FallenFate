using UnityEngine;

public class WardenSpawn : MonoBehaviour
{
    public GameObject Warden;
    public Transform targetSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Warden = GameObject.FindGameObjectWithTag("Warden");
        targetSpawnPoint = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Warden.transform.position = targetSpawnPoint.position;
        }
    }
}
