using UnityEngine;
using UnityEngine.AI;

public class Fly : MonoBehaviour
{
    private Transform self;
    private SimpleAi FlyAi;

    void Start()
    {
        self = GetComponent<Transform>();
        FlyAi = GetComponent<SimpleAi>();
        FlyAi.attackPointOffset = new Vector3(0, 1, 0);
    }

    void Update()
    {
        transform.position = transform.position + new Vector3(0, 1, 0);
    }
}
