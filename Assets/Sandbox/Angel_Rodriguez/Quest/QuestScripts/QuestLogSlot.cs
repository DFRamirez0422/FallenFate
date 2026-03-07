using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Script represents a UI slot for displaying a quest in the quest log.
public class QuestLogSlot : MonoBehaviour
{
    public QuestSO CurrentQuestSO;
    [SerializeField] private TMP_Text QuestNameText;

    [SerializeField] private GameObject DisplayNextQuestButton;

    
    public QuestLogUI questLogUI;

        void Start()
    {
        StartCoroutine(AutoUpdateQuestCoroutine());
        SetQuest(CurrentQuestSO);
    } 

    private void OnValidate()
    {
        if (CurrentQuestSO != null)
        SetQuest(CurrentQuestSO);
        else
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetButtonDown("Off/OnUI"))
        {
            AutoUpdateQuest();
        }
    }

    // This method sets the quest for this slot and updates the UI to display the quest name.
    public void SetQuest(QuestSO quest)
    {
        CurrentQuestSO = quest;
        QuestNameText.text = quest.questName;
        gameObject.SetActive(true);
    }
    
    void getCompletedStatus(QuestSO quest)
    {
          QuestEvents.IsQuestCompleted?.Invoke(quest);
          if(QuestEvents.IsQuestCompleted != null)
          {
            bool isCompleted = QuestEvents.IsQuestCompleted.Invoke(quest);
            if(isCompleted)
            {
                this.gameObject.GetComponent<Button>().interactable = false;
                this.gameObject.GetComponent<QuestLogSlot>().enabled = false;
                if(DisplayNextQuestButton != null){
                DisplayNextQuestButton.SetActive(true);
                this.gameObject.SetActive(false);
                }
                
            }
          }
    }


    //It tells the QuestLogUI to display the details of the selected quest.
    public void OnSlotClicked()
    {
        if (CurrentQuestSO != null)
        {
            questLogUI.HandleQusetClicked(CurrentQuestSO);
            getCompletedStatus(CurrentQuestSO);
        }
    }

    void AutoUpdateQuest()
    {
        if(CurrentQuestSO != null)
        {
            questLogUI.HandleQusetClicked(CurrentQuestSO);
        }
    }

     IEnumerator AutoUpdateQuestCoroutine()
    {
        while(true)        {
            yield return new WaitForSeconds(1.5f);
            AutoUpdateQuest();
        }
    }
}
