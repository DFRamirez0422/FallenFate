using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Scenes (must be added to Build Settings)")]
    [SerializeField] private string playSceneName;
    [SerializeField] private string creditsSceneName;

    [Header("Panels (assign in Inspector)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject keybindingsPanel;

    // Scene Loading for buttons
    public void Play()
    {
        LoadSceneByName(playSceneName);
    }

    public void Credits()
    {
        LoadSceneByName(creditsSceneName);
    }

    private void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[ButtonManager] Scene name is empty. Assign it in the Inspector.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    #region PANEL ACTIVATION FOR BUTTONS

    public void ActivateMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        soundPanel.SetActive(false);
        keybindingsPanel.SetActive(false);
    }

    public void ActivateOptionsPanel()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        soundPanel.SetActive(false);
        keybindingsPanel.SetActive(false);
    }

    public void ActivateSoundPanel()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        soundPanel.SetActive(true);
        keybindingsPanel.SetActive(false);
    }

    public void ActivateKeybindingsPanel()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        soundPanel.SetActive(false);
        keybindingsPanel.SetActive(true);
    }
    #endregion

    // Quit logic for buttons
    public void QuitGame()
    {
        Debug.Log("[ButtonManager] Quit pressed.");
        Application.Quit();
    }
}
