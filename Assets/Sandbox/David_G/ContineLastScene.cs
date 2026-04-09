using UnityEngine;

public class ContineLastScene : MonoBehaviour
{

    [SerializeField] private Scene_tracker m_SceneTracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_SceneTracker = FindObjectOfType<Scene_tracker>();
    }

    public void LoadPreviousScene()
    {
        m_SceneTracker.loadPreviousScene();
    }

}
