using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{    
    private CanvasGroup m_CanvasGroup;
    private bool m_SavedCursorVisible;
    private CursorLockMode m_SavedCursorLockState;

    void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }

    public void DisplayScreen()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;

        // Ensure the cursor is visible.
        m_SavedCursorVisible = Cursor.visible;
        m_SavedCursorLockState = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Disable all player and AI movement.
        // TODO: respawn points do not work at all if the time scale is set to zero.
        Time.timeScale = 0.0f;
    }

    public void HideScreen()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
        Cursor.visible = m_SavedCursorVisible;
        Cursor.lockState = m_SavedCursorLockState;
    }

    public void OnPressRetryButton()
    {
        // Re-enable all player and AI movement.
        Time.timeScale = 1.0f;

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
