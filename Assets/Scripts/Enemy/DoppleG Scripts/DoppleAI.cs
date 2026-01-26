using UnityEngine;

public class DoppleAI : MonoBehaviour
{
    private Waypoints waypointScript;
    private ObjectFader objectFader;

    private void Start()
    {
        waypointScript = GetComponent<Waypoints>();
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
            waypointScript.waypointIndex += 1;
        }

        if (waypointScript.waypointIndex == waypointScript.waypoints.Length)
        {
            objectFader.DoFade = true;
        }
    }
}
