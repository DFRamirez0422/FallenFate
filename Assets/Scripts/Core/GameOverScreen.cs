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
        m_CanvasGroup = GetComponent<CanvasGroup>();
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

        // Object should unalive itself once the retry button itself since it is decided it is merely an instance.
        Destroy(this.gameObject);
    }

    public void OnPressQuitButton()
    {
        Debug.Log("Game will now close on the application build.");
        Application.Quit();
    }

    public void RespawnPlayer()
    {
        // Very cheap hack to get around prefabs limitation of not invoking a callback of another prefab.
        // I know, I know, I know it burns, but it could be worse. At least the player is always alive.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().ResetPlayer();
    }
}
