using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{    
    // private CanvasGroup m_CanvasGroup;
    private bool m_SavedCursorVisible;
    private CursorLockMode m_SavedCursorLockState;

    // ===== PRIVATE FIELDS ===== //
    public static GameOverScreen Instance;
    private string m_DiedAtSceneName;
    private GameOverController m_GameOverController;

    void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     // GameObject old = Instance.gameObject;
        //     // Instance = this;
        //     // Destroy(old);
        //     Destroy(this.gameObject);
        // }

        m_GameOverController = GameObject.FindGameObjectWithTag("GameOverController").GetComponent<GameOverController>();

        // m_CanvasGroup = GetComponent<CanvasGroup>();
        // m_CanvasGroup.alpha = 0;
        // m_CanvasGroup.interactable = false;
        // m_CanvasGroup.blocksRaycasts = false;

        // Ensure the cursor is visible.
        // m_SavedCursorVisible = Cursor.visible;
        // m_SavedCursorLockState = Cursor.lockState;
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }

    public void StartGameOverScreen()
    {
        m_DiedAtSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("GameOverScene");
    }

    // public void DisplayScreen()
    // {
    //     m_CanvasGroup = GetComponent<CanvasGroup>();
    //     m_CanvasGroup.alpha = 1;
    //     m_CanvasGroup.interactable = true;
    //     m_CanvasGroup.blocksRaycasts = true;

    //     // Ensure the cursor is visible.
    //     m_SavedCursorVisible = Cursor.visible;
    //     m_SavedCursorLockState = Cursor.lockState;
    //     Cursor.visible = true;
    //     Cursor.lockState = CursorLockMode.None;
    // }

    // public void HideScreen()
    // {
    //     m_CanvasGroup = GetComponent<CanvasGroup>();
    //     m_CanvasGroup.alpha = 0;
    //     m_CanvasGroup.interactable = false;
    //     m_CanvasGroup.blocksRaycasts = false;
    //     Cursor.visible = m_SavedCursorVisible;
    //     Cursor.lockState = m_SavedCursorLockState;
    // }

    public void OnPressRetryButton()
    {
        Debug.Log("Current scene will restart!");
        Cursor.visible = m_SavedCursorVisible;
        Cursor.lockState = m_SavedCursorLockState;
        m_GameOverController.RespawnPlayer();
    }

    public void OnPressQuitButton()
    {
        Debug.Log("Game will now close on the application build.");
        Application.Quit();
    }
}
