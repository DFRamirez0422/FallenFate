using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    public Transform destination; // Where the player should end up
    public ScreenFader screenFader;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private System.Collections.IEnumerator TeleportPlayer(Transform player)
    {
        isTransitioning = true;

        // Fade to black
        screenFader.FadeToBlack(null);
        yield return new WaitForSeconds(screenFader.fadeDuration);

        // Move player
        player.position = destination.position;
        player.rotation = destination.rotation;

        // Fade back in
        screenFader.FadeFromBlack(null);
        yield return new WaitForSeconds(screenFader.fadeDuration);

        isTransitioning = false;
    }
}
