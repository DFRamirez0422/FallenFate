using UnityEngine;

public class TimeINIT : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

}
