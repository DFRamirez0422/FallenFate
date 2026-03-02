using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    public string sceneToLoad;
    [SerializeField] private UnityEvent m_OnSkipCutsceneEvent;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Input.GetKeyDown("Interact"))
        {
            m_OnSkipCutsceneEvent?.Invoke();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
