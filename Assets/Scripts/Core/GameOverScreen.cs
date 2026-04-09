using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{    
    // private CanvasGroup m_CanvasGroup;
    private bool m_SavedCursorVisible;
    private CursorLockMode m_SavedCursorLockState;

    void Awake()
    {
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
        RespawnPlayer();
    }

    public void OnPressQuitButton()
    {
        Debug.Log("Game will now close on the application build.");
        Application.Quit();
    }

    private void RespawnPlayer()
    {
        // Very cheap hack to get around prefabs limitation of not invoking a callback of another prefab.
        // I know, I know, I know it burns, but it could be worse. At least the player is always alive.
        //
        // 2026-04-09:
        // We have a list of persistent objects, right? "Correct."
        // And the player is one of them, right? "Yes."
        // So let's just save the scene name to the player because once you load a scene, there's no way to transfer
        // data except via persistent objects. "Sure, that makes sense."
        // Let's try to load the scene, then. "NullReferenceException"
        // Wait, why is there no persistent objects in this game anymore? "idk lol"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().ResetPlayer();
    }
}
