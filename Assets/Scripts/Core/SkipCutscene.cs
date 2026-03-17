using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    public string sceneToLoad;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            LoadScene(sceneToLoad);
        }

    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
