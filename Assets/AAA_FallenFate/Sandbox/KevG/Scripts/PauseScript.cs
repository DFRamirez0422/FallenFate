using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject Container;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
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
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Container.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
