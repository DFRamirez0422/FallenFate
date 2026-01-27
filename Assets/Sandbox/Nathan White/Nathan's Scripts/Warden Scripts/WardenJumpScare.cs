using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WardenJumpScare : MonoBehaviour
{
    public GameObject jumpscareImage;
    public AudioClip jumpscareClip1;
    public AudioSource jumpscareSource;

    //scripts
    private PlayerHealth health;
    private WardenMovement movement;
    private Disarm disarm;

    public GameObject TextCanvas; // can be removed later if we have a different way to indicate damage
    public TextMeshProUGUI text;

    private void Start()
    {
        movement = GetComponent<WardenMovement>();
        disarm = GetComponent<Disarm>();

        jumpscareImage.SetActive(false);
        TextCanvas.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        health = collision.gameObject.GetComponent<PlayerHealth>();
        Debug.Log("Collided with: " + collision.gameObject.name);

        if (health != null)
        {
            if (health.CurrentHealth > 0)
            {
                health.ChangeHealth(-2);
                movement.stunned = true;
                text.text = "Damaged";
                TextCanvas.SetActive(true);
                StartCoroutine(HideTextCanvasDamage());
            }
            if (health.CurrentHealth <= 0)
            {
                jumpscareImage.SetActive(true); Debug.Log("Enabled Image");

                if (jumpscareClip1 != null)
                {
                    jumpscareSource.PlayOneShot(jumpscareClip1);
                }
                StartCoroutine(CloseJumpscare());
            }
        }



        if (collision.gameObject.tag == "Sword") //can use another tag other than sword (Like attack or swing)
        {
            if (movement.stunned == false)
            {
                movement.stunned = true;
                text.text = "Stunned";
                TextCanvas.SetActive(true);
                StartCoroutine(HideTextCanvasStunned());
            }
            else if (movement.stunned == true)
            {
                text.text = "Disarmed you";
                disarm.swordOBJ = collision.gameObject;
                disarm.disarmed = true;
                StartCoroutine(CallRearm());
            }
        }

    }

    private IEnumerator CloseJumpscare()
    {
        yield return new WaitForSeconds(2);
        jumpscareImage.SetActive(false);
    }

    private IEnumerator HideTextCanvasDamage()
    {
        yield return new WaitForSeconds(2);
        TextCanvas.SetActive(false);
    }

    private IEnumerator HideTextCanvasStunned()
    {
        yield return new WaitForSeconds(2);
        TextCanvas.SetActive(false);
    }
    private IEnumerator CallRearm()
    {
        yield return new WaitForSeconds(2);
        disarm.Rearm();
    }
}
