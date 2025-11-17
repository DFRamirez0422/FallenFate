using UnityEngine;
using UnityEngine.SceneManagement;

public class TMP_BOSS_DEATH : MonoBehaviour
{
    public GameObject boss;

    void Update()
    {
        if (boss == null)
        {
            SceneManager.LoadScene("Credits");
        }
    }
}
