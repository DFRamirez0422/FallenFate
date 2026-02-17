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
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            objectFader.DoFade = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            objectFader.DoFade = false;
        }
    }
}
