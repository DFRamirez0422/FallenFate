using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// High level game manager that marks all stationed game objects as well as manage persistent data
    /// that cannot be reset between scene loads and may bee loaded in by an external save file.
    /// </summary>

    // Singleton instance.
    public static GameManager s_Instance;

    [Header("Persistent Objects")]
    [SerializeField] private GameObject[] m_PersistentObjects;

    private void Awake()
    {
        if (s_Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void MarkPersistentObjects()
    {
        foreach(GameObject obj in m_PersistentObjects)
        {
            if(obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach(GameObject obj in m_PersistentObjects)
        {
            Destroy(obj);
        }

        Destroy(gameObject);
    }
}
