using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class WardenJumpScare : MonoBehaviour
{
    public VideoPlayer jumpscare;

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
        jumpscare.Play();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    health = collision.gameObject.GetComponent<PlayerHealth>();
    //    Debug.Log("Collided with: " + collision.gameObject.name);

    //    //if (health != null)
    //    //{
    //    //    if (health.CurrentHealth > 0)
    //    //    {
    //    //        health.ChangeHealth(-2);
    //    //        movement.stunned = true;
    //    //        text.text = "Damaged";
    //    //        TextCanvas.SetActive(true);
    //    //        StartCoroutine(HideTextCanvasDamage());
    //    //    }

    //    //    This is the thing that starts the jumpscare
    //    //    if (health.CurrentHealth <= 0)
    //    //    {
    //    //        jumpscareImage.SetActive(true); Debug.Log("Enabled Image");

    //    //        if (jumpscareClip1 != null)
    //    //        {
    //    //            jumpscareSource.PlayOneShot(jumpscareClip1);
    //    //        }
    //    //        StartCoroutine(CloseJumpscare());
    //    //    }
    //    //}

    //}

    public void Taunt()
    {
        TextCanvas.SetActive(true);
        int rando = Random.Range(1, 4);

        if (rando == 1)
        {
            text.text = "Useless";
        }
        else if (rando == 2)
        {
            text.text = "Weak";
        }
        else if (rando == 3)
        {
            text.text = "Pitiful";
        }
        else if (rando == 4)
        {
            text.text = "Futile";
        }

        Invoke(nameof(HideTextCanvas), 0.5f);
    }

    public void playJumpscare()
    {

    }
    

private IEnumerator CloseJumpscare()
    {
        yield return new WaitForSeconds(2);
    }

    public void HideTextCanvas()
    {
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
