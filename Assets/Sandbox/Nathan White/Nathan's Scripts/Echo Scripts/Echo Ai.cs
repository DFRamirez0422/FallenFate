using System.Transactions;
using UnityEngine;

public class EchoAi : MonoBehaviour
{
    private Transform player;
    private ObjectFader objectFader;

    public bool FaceAway, Started;
    public EchoWaypoint EchoWaypointScript;
    private AudioSource EchoDeathScream;

    private void Start()
    {
        Started = true;
        objectFader = GetComponent<ObjectFader>();
        EchoWaypointScript = GetComponent<EchoWaypoint>();
        EchoDeathScream = GetComponent<AudioSource>();
    }

    private void Update()
    {

        if (objectFader.Mat.color.a <= 0.01f)
        {
            SelfDestroy();
        }

        if (FaceAway)
        {
            Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
            // Calculate the direction from the trigger (this.transform.position) 
            // to the other object (other.transform.position)
            Vector2 directionToTarget = player.transform.position - this.transform.position;

            // Normalize the vector to get only the direction with a magnitude (length) of 1
            Vector2 normalizedDirection = directionToTarget.normalized;

            // You can now use normalizedDirection for various purposes
            //Debug.Log("Direction to " + collision.gameObject.name + ": " + normalizedDirection);

            if (normalizedDirection.x < -0.1f)
            {
                // Resets to face forward when moving right.  Basically face right
                transform.eulerAngles = new Vector2(0, 0);
            }
            else if (normalizedDirection.x > 0.1f)
            {
                // Sets the rotation exactly to 0, 180, 0.  Basically face left
                transform.eulerAngles = new Vector2(0, 180);
            }
        }

    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
    private void PlayDeath()
    {
        EchoDeathScream.Play();
    }
}
