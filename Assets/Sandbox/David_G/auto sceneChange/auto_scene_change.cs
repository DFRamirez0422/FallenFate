using UnityEngine;
using UnityEngine.SceneManagement;

public class auto_scene_change : MonoBehaviour
{

    public string sceneToLoad;
    void Awake()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
