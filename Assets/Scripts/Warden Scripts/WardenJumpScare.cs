using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WardenJumpScare : MonoBehaviour
{
    public GameObject jumpscareImage;
    public AudioClip jumpscareClip1;
    public AudioSource jumpscareSource;

    private SplayerHealth health;
    private WardenMovement movement;

    public GameObject DamageCanvas; // can be removed later if we have a different way to indicate damage

    private void Start()
    {
        movement = GetComponent<WardenMovement>();
        jumpscareImage.SetActive(false);
        DamageCanvas.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        health = collision.gameObject.GetComponent<SplayerHealth>();
        Debug.Log("Collided with: " + collision.gameObject.name);

        if (health.currentHealth > 0)
        {
            health.ChangeHealth(-2);
            movement.stunned = true;
            DamageCanvas.SetActive(true);
            StartCoroutine(HideDamageText());
        }
        if (health.currentHealth <= 0)
        {
            jumpscareImage.SetActive(true); Debug.Log("Enabled Image");

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
        jumpscareImage.SetActive(false);
    }

    private IEnumerator HideDamageText()
    {
        yield return new WaitForSeconds(2);
        DamageCanvas.SetActive(false);
    }
}
