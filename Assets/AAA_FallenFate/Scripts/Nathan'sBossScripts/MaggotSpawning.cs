using System;
using System.Collections;
using UnityEngine;

public class MaggotSpawning : MonoBehaviour
{
    public GameObject Fly;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }
}
