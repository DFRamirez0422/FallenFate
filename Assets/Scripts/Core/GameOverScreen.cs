using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;

    void Awake()
    {
        HideScreen();
    }

    public void DisplayScreen()
    {
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    public void HideScreen()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }

    public void OnPressRetryButton()
    {
        Debug.Log("Current scene will restart!");
        HideScreen();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPressQuitButton()
    {
        Debug.Log("Game will now close on the application build.");
        Application.Quit();
    }
}
