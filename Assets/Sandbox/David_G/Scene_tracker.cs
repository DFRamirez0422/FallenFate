using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_tracker : MonoBehaviour
{
     public string currentScene;
     private string previousScene;
     private string sceneToLoad;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene != "GameOver_NEW")
        {
            previousScene = currentScene;
        }
        else if (currentScene == "GameOver_NEW")
        {
            sceneToLoad = previousScene;
        }

    }

    public void loadPreviousScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
