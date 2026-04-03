using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    /// <summary>
    /// This allows for a designer to specify what type of action to take upon either ending dialogue
    /// or selecting an option.
    /// </summary>
    public enum ActionOnEnd
    {
        EndDialogue, // Simply ends a dialogue tree with no other side effects.
        NewDialogue, // Branch to a new dialogue tree.
        ChangeScene, // Changes to a new scene given by a parameter.
        SetObjectsActive, // Activates a list of game objects in the scene.
        InstantiateObjects, // Instantiates a list of prefabs into the scene.
    }

    public DialogueLine[] lines;
    public DialogueOption[] options;

    [Header("Conditional Requirements (Optional)")]
    public ActorSO[] requiredNPCs;

    [Header("Action upon Dialogue Ending")]
    [Tooltip("What action to take upon the dialogue ending.")]
    public Action actionOnDialogueEnd = Action.EndDialogue;

    [Tooltip("If 'ChangeScene' was set, the name of the scene to load.")]
    public string sceneName;
    
    [Tooltip("If 'NewDialogue' was set, branch to a new dialogue tree upon selection.")]
    public DialogueSO nextDialogue;

    [Tooltip("If 'SetObjectActive' was set, the list of all game objects to activate.")]
    public GameObject[] objectsToActivate;

    [Tooltip("If 'InstantiateObject' was set, the list of all game object prefabs to spawn.")]
    public GameObject[] objectsToInstantiate;

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
        ChangeScene,
    }

    public string optionText;
    public Action action = Action.NewDialogue;
    [Tooltip("If 'NewDialogue' was set, branch to a new dialogue tree upon selection.")]
    public DialogueSO nextDialogue;
    [Tooltip("If 'ChangeScene' was set, the name of the scene to load.")]
    public string sceneName;
}