using UnityEngine;
using UnityEngine.SceneManagement;

public class DG_scene_change : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
