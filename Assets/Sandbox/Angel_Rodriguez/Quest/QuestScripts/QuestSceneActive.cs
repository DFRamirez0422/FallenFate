using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuestSceneActive : MonoBehaviour
{
    [SerializeField] private string scene1;
    [SerializeField] private string scene2;
    //[SerializeField] private string scene3;
    
    [SerializeField] private GameObject QuestSet1;
    [SerializeField] private GameObject QuestSet2;
    //[SerializeField] private GameObject QuestSet3;
    
    [SerializeField] TMP_Text QuestAreaNameText;
     
    //private bool InRoom3;
    void FixedUpdate()
    {
        CheckScene();
    }
    private void CheckScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(scene1) && activeSceneName == scene1)
        {
            QuestAreaNameText.text = "Prison Docks";
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(true);
            QuestSet2.SetActive(false);
            //QuestSet3.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(scene2) && activeSceneName == scene2)
        {
            QuestAreaNameText.text = "Prison Cells/OutDoors";
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(false);
            QuestSet2.SetActive(true);
            //QuestSet3.SetActive(false);
        }
    }
}