using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class SceneLoader : MonoBehaviour
{
public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu1");

    }


    public void LoadLevel1()
    {
        SceneManager.LoadScene("L3_Bargaining_Infirmary");
    }


    public void LoadHelpScene()
    {
        SceneManager.LoadScene("Help");
    }


    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
