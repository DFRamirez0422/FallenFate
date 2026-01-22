using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;
    public DialogueOption[] options;

    [Header("Conditional Requirements (Optional)")]
    public ActorSO[] requiredNPCs;

    // items
    // locations

    public bool IsConditionMet()
    {
        if (requiredNPCs.Length > 0)
        {
            foreach(var npc in requiredNPCs)
            {
                if (!DialogueHistoryTracker.Instance.HasSpokenWith(npc))
                {
                    return false;
                }
            }
        }

        // check for items
        // check for locations

        return true;
    }
}

[System.Serializable]
public class DialogueLine
{
    public ActorSO speaker;
    [TextArea(3,5)] public string text;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSO nextDialogue;
}