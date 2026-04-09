using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameOverScene : MonoBehaviour
{ 
    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver_NEW");
    }

}
