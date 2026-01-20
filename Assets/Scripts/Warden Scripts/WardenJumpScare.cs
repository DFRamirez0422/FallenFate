using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WardenJumpScare : MonoBehaviour
{
    public Image jumpscareImage;
    public AudioClip jumpscareClip1;
    public AudioSource jumpscareSource;

    private SplayerHealth health;
    private WardenMovement movement;

    private void Start()
    {
        movement = GetComponent<WardenMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        health = collision.gameObject.GetComponent<SplayerHealth>();
        Debug.Log("Collided with: " + collision.gameObject.name);

        if (health.currentHealth > 0)
        {
            health.ChangeHealth(-2);
            movement.stunned = true;
        }
        if (health.currentHealth <= 0)
        {
            jumpscareImage.enabled = true; Debug.Log("Enabled Image");

            if (jumpscareClip1 != null)
            {
                jumpscareSource.PlayOneShot(jumpscareClip1);
            }
            StartCoroutine(CloseJumpscare());
        }

        
    }

    private IEnumerator CloseJumpscare()
    {
        yield return new WaitForSeconds(2);
        jumpscareImage.enabled = false;
    }
}
