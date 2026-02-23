using TMPro;
using UnityEngine;

// Script represents a UI slot for displaying a quest objective in the quest log.
public class QuestObjectiveSlot : MonoBehaviour
{
   [SerializeField] private TMP_Text ObjectiveText;
   [SerializeField] private TMP_Text TrackingText;

    // This method refreshes the objective slot with the latest information about the quest objective
    // including its description, progress text, and completion status.
    public void RefreshObjective(QuestObjective objective, string progressText, bool isCompleted)
    {
        ObjectiveText.text = objective.Description;
        TrackingText.text = progressText;

        Color textColor = isCompleted ? Color.gray : Color.white;
        ObjectiveText.color = textColor;
        TrackingText.color = textColor;
    }
}
