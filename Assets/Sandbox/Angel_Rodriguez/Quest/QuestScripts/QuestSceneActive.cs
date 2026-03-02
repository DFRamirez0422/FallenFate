using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuestSceneActive : MonoBehaviour
{
    [SerializeField] private SceneAsset scene1;
    [SerializeField] private SceneAsset scene2;
    //[SerializeField] private SceneAsset scene3;
    
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
        if (scene1 != null && activeSceneName == scene1.name)
        {
            QuestAreaNameText.text = "Prison Docks";
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(true);
            QuestSet2.SetActive(false);
            //QuestSet3.SetActive(false);
        }
        else if (scene2 != null && activeSceneName == scene2.name)
        {
            QuestAreaNameText.text = "Prison Cells/OutDoors";
            QuestAreaNameText.color = Color.red;
            QuestSet1.SetActive(false);
            QuestSet2.SetActive(true);
            //QuestSet3.SetActive(false);
        }
    }
}
