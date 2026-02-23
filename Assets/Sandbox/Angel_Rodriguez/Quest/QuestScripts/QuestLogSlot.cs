using TMPro;
using UnityEngine;

// Script represents a UI slot for displaying a quest in the quest log.
public class QuestLogSlot : MonoBehaviour
{
    public QuestSO CurrentQuestSO;
    [SerializeField] private TMP_Text QuestNameText;
    
    public QuestLogUI questLogUI;
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
    
    //It tells the QuestLogUI to display the details of the selected quest.
    public void OnSlotClicked()
    {
        if (CurrentQuestSO != null)
        {
            questLogUI.HandleQusetClicked(CurrentQuestSO);
        }
    }

    void AutoUpdateQuest()
    {
        if(CurrentQuestSO != null)
        {
            questLogUI.HandleQusetClicked(CurrentQuestSO);
        }
    }
}
