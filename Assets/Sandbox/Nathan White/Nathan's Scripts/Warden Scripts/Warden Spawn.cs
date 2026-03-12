using Unity.Android.Gradle.Manifest;
using UnityEditor.UI;
using UnityEngine;

public class WardenSpawn : MonoBehaviour
{
    public GameObject Warden;
    public Transform targetSpawnPoint;
    public static bool reset;
    public bool activeTP = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Warden = GameObject.FindGameObjectWithTag("Warden");
        targetSpawnPoint = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (reset == true)
        {
            activeTP = true;
            Debug.Log("Spawners reset");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && activeTP == true)
        {
            reset = true;
            Debug.Log("1");
            Warden.transform.position = targetSpawnPoint.position;
            Invoke(nameof(disabletp), 1);
            Debug.Log("3");
        }
    }

    void disabletp()
    {
        reset = false;
        activeTP = false;
    }
}
