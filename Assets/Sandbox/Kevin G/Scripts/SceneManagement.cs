using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class SceneManagerUnity : MonoBehaviour
{
    public void LoadlEVEL1()
    {
        SceneManager.LoadScene("Act 1 REMAKE");

    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

