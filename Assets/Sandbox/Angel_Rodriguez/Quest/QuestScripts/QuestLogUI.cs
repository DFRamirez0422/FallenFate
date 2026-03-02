using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public class QuestLogUI : MonoBehaviour
{
    // This script is responsible for displaying the quest log UI and updating it based on the player's progress.
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text QuestNameText;
    [SerializeField] private TMP_Text QuestDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;

    bool isCompleted;

    private QuestSO questSO;

    // This method is called when the quest button is clicked in the quest log. 
    // It updates the UI to show the details of the selected quest.
    public void HandleQusetClicked(QuestSO quest)
    {
        this.questSO = quest;
        QuestNameText.text = quest.questName;
        QuestDescriptionText.text = quest.Questdescription;
         
        DisplayOgjective();

        foreach (var objective in quest.objectives)
        {
            Debug.Log($"Objective: {objective.Description} => Progress: {questManager.GetProgressText(quest, objective)}");
        }
    }

    // This method updates the objective slots in the UI based on the current progress of the quest objectives.
    public void DisplayOgjective()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSO.objectives.Count)
            {
                var objective = questSO.objectives[i];
                questManager.updateQuestProgress(questSO, objective);
                int currentProgress = questManager.GetCurrentProgress(questSO, objective);
                string progressText = questManager.GetProgressText(questSO, objective);
                isCompleted = currentProgress >= objective.requiredAmount;
                questManager.IsQuestComplete(questSO);
                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjective(objective, progressText, isCompleted);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
