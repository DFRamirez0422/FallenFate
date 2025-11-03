using UnityEngine;
using UnityEngine.AI;

public class Fly : MonoBehaviour
{
    private Transform self;

    void Start()
    {
        self = GetComponent<Transform>();
    }

    void Update()
    {
        transform.position = transform.position + new Vector3(0, 1, 0);
    }
}
