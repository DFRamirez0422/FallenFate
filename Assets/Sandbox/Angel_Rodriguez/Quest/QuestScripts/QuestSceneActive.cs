using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuestSceneActive : MonoBehaviour
{
    [SerializeField] private SceneAsset scene1;
    [SerializeField] private SceneAsset scene2;
    [SerializeField] private SceneAsset scene3;
    
    [SerializeField] private GameObject QuestSet1;
    [SerializeField] private GameObject QuestSet2;
    [SerializeField] private GameObject QuestSet3;

    [SerializeField] private string QuestAreaName1;
    [SerializeField] private string QuestAreaName2;
    [SerializeField] private string QuestAreaName3;
    
    [SerializeField] TMP_Text QuestAreaNameText;
     
    //private bool InRoom3;
    void FixedUpdate()
    {
        CheckScene();
    }
    private void CheckScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == scene1.name)
        {
            QuestAreaNameText.text = QuestAreaName1;
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(true);
            QuestSet2.SetActive(false);
            QuestSet3.SetActive(false);
        }
        else if (activeSceneName == scene1.name)
        {
            QuestAreaNameText.text = QuestAreaName2;
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(false);
            QuestSet2.SetActive(true);
            QuestSet3.SetActive(false);
        }
        else if(activeSceneName == scene3.name)
        {
            QuestAreaNameText.text = QuestAreaName3;
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(false);
            QuestSet2.SetActive(false);
            QuestSet3.SetActive(true);
        }
    }
}