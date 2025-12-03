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
        SceneManager.LoadScene("L3_Bargain");
    }


    public void LoadHelpScene()
    {
        SceneManager.LoadScene("Help");
    }


    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadTitlecard()
    {
        SceneManager.LoadScene("TitleCard");
    }
}
