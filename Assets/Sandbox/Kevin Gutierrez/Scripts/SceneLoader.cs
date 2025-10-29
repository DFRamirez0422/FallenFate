using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class SceneLoader : MonoBehaviour
{
public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu1");

    }


    public void LoadLevel1()
    {
     // COMPILATION ERROR   SceneManager.LoadScene("")
    }
}
