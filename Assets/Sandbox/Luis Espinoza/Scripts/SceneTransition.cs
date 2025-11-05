using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Settings")]
    public Animator transition;      
    public float transitionTime = 1f;
    public string nextScene;         

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadScene(nextScene));
    }

    System.Collections.IEnumerator LoadScene(string sceneName)
    {
        if (transition != null)
            transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName); 
    }
}