using UnityEngine;
using System.Collections;

public class MaggotSpawning : MonoBehaviour
{
    public float SpawnRate = 20;
    public GameObject Fly; //The is what gets spawned. Should be switched out with a proper enemy as it only has a template.
    private bool Spawning = false;
    private Transform self;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<Transform>();
        Spawning = true;
        StartCoroutine(SpawnFly());
    }

    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator SpawnFly()
    {
        Instantiate(Fly, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(20);
        while (Spawning)
        {
            GameObject.Instantiate(Fly, self.position, Quaternion.identity);
            yield return new WaitForSeconds(SpawnRate);
        }

    }
}