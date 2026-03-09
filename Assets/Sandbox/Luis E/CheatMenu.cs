using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // IMPORTANT for TextMeshPro

public class CheatMenu : MonoBehaviour
{
    [System.Serializable]
    public class ObjectCheat
    {
        public string name;        // Label for the button
        public GameObject target;  // GameObject to toggle
        public Button button;      // Assign in Inspector
    }

    [System.Serializable]
    public class SceneCheat
    {
        public string name;       // Label for the button
        public string sceneName;  // Scene to load
        public Button button;     // Assign in Inspector
    }

    [Header("Cheats")]
    public ObjectCheat[] objectCheats;
    public SceneCheat[] sceneCheats;

    [Header("UI")]
    public GameObject cheatMenuCanvas; // Canvas or panel for the menu

    private void Start()
    {
        cheatMenuCanvas.SetActive(false);

        // Setup Object Cheats buttons
        foreach (var cheat in objectCheats)
        {
            if (cheat.button != null)
            {
                // Set TextMeshPro text
                TMP_Text txt = cheat.button.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                    txt.text = cheat.name;

                // Remove previous listeners and add listener
                cheat.button.onClick.RemoveAllListeners();
                cheat.button.onClick.AddListener(() =>
                {
                    if (cheat.target != null)
                        cheat.target.SetActive(!cheat.target.activeSelf);
                });
            }
        }

        // Setup Scene Cheats buttons
        foreach (var cheat in sceneCheats)
        {
            if (cheat.button != null)
            {
                // Set TextMeshPro text
                TMP_Text txt = cheat.button.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                    txt.text = cheat.name;

                // Remove previous listeners and add listener
                cheat.button.onClick.RemoveAllListeners();
                cheat.button.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(cheat.sceneName))
                        SceneManager.LoadScene(cheat.sceneName);
                });
            }
        }
    }

    private void Update()
    {
        // Toggle the menu with backquote `
        if (Input.GetKeyDown(KeyCode.BackQuote))
            cheatMenuCanvas.SetActive(!cheatMenuCanvas.activeSelf);
    }
}