using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room2_CutScene_Player : MonoBehaviour
{
    [SerializeField] private List<Texture2D> cutsceneTextures;
    [SerializeField] private GameObject CutsceneCanvas;
    [SerializeField] private float frameDuration; // Duration of each frame in seconds
    [SerializeField]private PlayerMovement playerMovement;
    
    void Awake()
    {
        CutsceneCanvas = Resources.Load<GameObject>("Prefabs/Room2_CutScenePrefabs/Room2CutScene");
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (CutsceneCanvas == null)
        {
            Debug.LogError("CutsceneCanvas not found in the scene.");
        }
    }

    private IEnumerator<WaitForSeconds> CutsceneCoroutine()
    { 
        foreach (var texture in cutsceneTextures)
        {
             // Display the texture (implementation depends on your UI setup)
             CutsceneCanvas.GetComponentInChildren<RawImage>().texture = texture; // Assuming you have a RawImage component on the canvas to display the texture

             // For example, you might set it to a RawImage component

             yield return new WaitForSeconds(frameDuration);
        }
        // After the cutscene, you can load the next scene or perform any other actions
        StopCoroutine(CutsceneCoroutine());
        CutsceneCanvas.SetActive(false); // Hide the cutscene canvas after the cutscene is done
        playerMovement.enabled = true; // Re-enable player movement after the cutscene
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovement.enabled = false; // Disable player movement during the cutscene
            CutsceneCanvas = Instantiate(CutsceneCanvas); // Instantiate the cutscene canvas
            CutsceneCanvas.SetActive(true); // Show the cutscene canvas
            StartCoroutine(CutsceneCoroutine());
            GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent retriggering
        }
    }
}
