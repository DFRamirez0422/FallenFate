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
    public ActorSO.Emotion emotion;
    [TextArea(3,5)] public string text;
}

[System.Serializable]
public class DialogueOption
{
    /// <summary>
    /// This allows for a designer to specify what type of action to take upon the player clicking a dialogue option.
    /// </summary>
    public enum Action
    {
        // Branch to a new dialogue tree upon selection.
        NewDialogue,
        // Change to a new scene upon selection.
        SceneChange,
    }

    public string optionText;
    public Action action;
    public DialogueSO nextDialogue;
    public string sceneName;
}