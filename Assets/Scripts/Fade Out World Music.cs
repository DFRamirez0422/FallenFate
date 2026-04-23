using UnityEngine;
using System.Collections;

public class FadeOutWorldMusic : MonoBehaviour
{
    public AudioSource theMusic;
    public float FadeTime;

    void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float startVolume = theMusic.volume;

        while (theMusic.volume > 0)
        {
            theMusic.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        theMusic.Stop();
        theMusic.volume = startVolume;
    }
}
