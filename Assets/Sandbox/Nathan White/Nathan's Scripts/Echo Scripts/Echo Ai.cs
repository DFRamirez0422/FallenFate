using UnityEngine;

public class EchoAi : MonoBehaviour
{
    private Transform player;
    private ObjectFader objectFader;

    private void Start()
    {
        objectFader = GetComponent<ObjectFader>();
    }

    private void Update()
    {

        if (objectFader.Mat.color.a <= 0.001f)
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
