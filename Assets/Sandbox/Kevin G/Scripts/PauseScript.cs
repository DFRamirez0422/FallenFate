using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject Container;      // Pause menu panel
    public GameObject OptionsPanel;   // Options panel

    private bool isPaused;

    void Start()
    {
        isPaused = false;

        // Make sure options is hidden at start
        if (OptionsPanel != null)
            OptionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // If options is open → go back to pause menu first
                if (OptionsPanel.activeSelf)
                {
                    BackToPause();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Container.SetActive(true);
        OptionsPanel.SetActive(false);

        Time.timeScale = 0.01f; // (you can change to 0 if you want full pause)

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        Container.SetActive(false);
        OptionsPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        isPaused = false;
    }

    // 🔹 NEW: Open Options
    public void OpenOptions()
    {
        Container.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    // 🔹 NEW: Back button
    public void BackToPause()
    {
        OptionsPanel.SetActive(false);
        Container.SetActive(true);
    }
}