using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    // ===== PRIVATE FIELDS ===== //
    public static GameOverController Instance;
    private string m_DiedAtSceneName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // GameObject old = Instance.gameObject;
            // Instance = this;
            // Destroy(old);
            Destroy(this.gameObject);
        }
    }

    public void StartGameOverScreen()
    {
        m_DiedAtSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("GameOverScene");
    }

    public void RespawnPlayer()
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
        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        // player.GetComponent<PlayerMovement>().ResetPlayer();
        SceneManager.LoadScene(m_DiedAtSceneName);
    }
}
