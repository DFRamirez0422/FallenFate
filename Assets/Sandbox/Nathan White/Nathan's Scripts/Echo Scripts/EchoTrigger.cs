using UnityEngine;

public class EchoTrigger : MonoBehaviour
{
    private ObjectFader objectFader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectFader = GetComponentInParent<ObjectFader>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
