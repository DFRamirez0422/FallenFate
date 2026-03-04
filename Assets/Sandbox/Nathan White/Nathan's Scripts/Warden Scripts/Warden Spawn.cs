using UnityEngine;

public class WardenSpawn : MonoBehaviour
{
    public GameObject Warden;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Warden = GameObject.FindGameObjectWithTag("Warden");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Warden.transform.position = transform.position;
    }
}
