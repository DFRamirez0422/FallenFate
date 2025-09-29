using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public string sceneName;
    public AudioSource audioSource;
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAfterAudio(sceneName));
    }
    
    public void LoadScene(string sceneName, AudioSource audio)
    {
        audioSource = audio;
        StartCoroutine(LoadSceneAfterAudio(sceneName));
    }

    public IEnumerator LoadSceneAfterAudio(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        
        if (audioSource != null)
        {
            Debug.Log("AudioSource found. Is playing: " + audioSource.isPlaying);
            
            // Start playing the audio if it's not already playing
            if (!audioSource.isPlaying)
            {
                Debug.Log("Starting audio playback...");
                audioSource.Play();
            }
            
            // Wait for audio to finish
            Debug.Log("Waiting for audio to finish...");
            yield return new WaitUntil(() => !audioSource.isPlaying);
            Debug.Log("Audio finished playing");
        }
        else
        {
            Debug.Log("No AudioSource assigned, loading scene immediately");
        }
        
        Debug.Log("Scene loaded: " + sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }


}
