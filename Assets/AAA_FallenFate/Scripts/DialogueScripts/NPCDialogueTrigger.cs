using System.Collections.Generic;
using UnityEngine;

public enum PostOneTimePriority
{
    OneTimeFirst,
    FloatingFirst
}

[System.Serializable]
public class ObjectInteraction
{
    [Tooltip("The BoxCollider trigger of the object (must be 'Is Trigger').")]
    public BoxCollider interactTrigger;

    [Header("Dialogue Content")]
    public Dialogue[] oneTimeDialogues;
    public Dialogue[] repeatableDialogues;
    [TextArea(2, 5)]
    public string[] floatingLines;

    [Header("Interaction Settings")]
    public PostOneTimePriority postOneTimePriority = PostOneTimePriority.OneTimeFirst;
    public bool playFloatingInOrder = false;

    [HideInInspector] public int nextOneTimeIndex = 0;
    [HideInInspector] public int nextRepeatableIndex = 0;
    [HideInInspector] public int lastRandomFloating = -1;
    [HideInInspector] public int lastOrderedFloating = -1;
    [HideInInspector] public bool oneTimePlayedAll = false;
    [HideInInspector] public bool floatingPlayedAll = false;
}

[RequireComponent(typeof(BoxCollider))]
public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Assign NPC")]
    public GameObject targetNPC;
    public BoxCollider npcTriggerCollider;

    [Header("NPC Dialogue Content")]
    public Dialogue[] npcOneTimeDialogues;
    public Dialogue[] npcRepeatableDialogues;
    [TextArea(2, 5)]
    public string[] npcFloatingLines;

    [Header("Dialogue Priority Settings")]
    public PostOneTimePriority postOneTimePriority = PostOneTimePriority.OneTimeFirst;
    public bool npcPlayFloatingInOrder = false;

    [Header("References")]
    public DialogueController dialogueController;
    public FloatingDialogue floatingDialogueComponent;
    public CombatAwareness combatAwarenessComponent;

    [Header("Object Interactions")]
    public List<ObjectInteraction> objectInteractions = new List<ObjectInteraction>();

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Detection Settings")]
    [Tooltip("How far from the trigger center the player can be to interact.")]
    public float interactDistance = 0.5f;

    // Internal NPC indices
    private int npcNextOneTime = 0;
    private int npcNextRepeatable = 0;
    private int npcLastRandomFloating = -1;
    private int npcLastOrderedFloating = -1;
    private bool npcOneTimePlayedAll = false;
    private bool npcFloatingPlayedAll = false;

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // First check object interactions
            foreach (var obj in objectInteractions)
            {
                if (obj.interactTrigger == null) continue;
                if (IsPlayerInTrigger(obj.interactTrigger, obj.interactTrigger.gameObject))
                {
                    TriggerObjectDialogue(obj);
                    return;
                }
            }

            // Then check NPC collider
            if (npcTriggerCollider != null && IsPlayerInTrigger(npcTriggerCollider, targetNPC))
            {
                TriggerNPCDialogue();
            }
        }
    }

    private bool IsPlayerInTrigger(BoxCollider trigger, GameObject target)
    {
        if (trigger == null || target == null) return false;
        Collider[] hits = Physics.OverlapBox(trigger.bounds.center, trigger.bounds.extents, trigger.transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }
        return false;
    }

    #region NPC Dialogue
    private void TriggerNPCDialogue()
    {
        // NPC-only Combat Awareness
        if (combatAwarenessComponent != null && combatAwarenessComponent.EnemiesNearby())
        {
            string nearest = combatAwarenessComponent.GetNearestEnemyTag(transform.position);
            if (!string.IsNullOrEmpty(nearest))
                combatAwarenessComponent.ShowCombatInterrupt(nearest);
            return;
        }

        if (dialogueController != null && dialogueController.IsDialogueActive()) return;

        switch (postOneTimePriority)
        {
            // One-Time > Repeatable > Floating
            case PostOneTimePriority.OneTimeFirst:
                if (!npcOneTimePlayedAll && npcOneTimeDialogues != null && npcNextOneTime < npcOneTimeDialogues.Length)
                {
                    PlayDialogue(npcOneTimeDialogues[npcNextOneTime]);
                    npcNextOneTime++;
                    if (npcNextOneTime >= npcOneTimeDialogues.Length)
                        npcOneTimePlayedAll = true;
                }
                else if (npcRepeatableDialogues != null && npcRepeatableDialogues.Length > 0)
                {
                    PlayDialogue(npcRepeatableDialogues[npcNextRepeatable]);
                    npcNextRepeatable = (npcNextRepeatable + 1) % npcRepeatableDialogues.Length;
                }
                else
                {
                    ShowFloatingDialogue(npcFloatingLines, ref npcLastRandomFloating, ref npcLastOrderedFloating, npcPlayFloatingInOrder, false, ref npcFloatingPlayedAll);
                }
                break;

            // Floating > One-Time > Repeatable
            case PostOneTimePriority.FloatingFirst:
                if (npcFloatingLines != null && npcFloatingLines.Length > 0)
                {
                    ShowFloatingDialogue(npcFloatingLines, ref npcLastRandomFloating, ref npcLastOrderedFloating, true, true, ref npcFloatingPlayedAll);
                }
                else if (!npcOneTimePlayedAll && npcOneTimeDialogues != null && npcNextOneTime < npcOneTimeDialogues.Length)
                {
                    PlayDialogue(npcOneTimeDialogues[npcNextOneTime]);
                    npcNextOneTime++;
                    if (npcNextOneTime >= npcOneTimeDialogues.Length)
                        npcOneTimePlayedAll = true;
                }
                else if (npcRepeatableDialogues != null && npcRepeatableDialogues.Length > 0)
                {
                    PlayDialogue(npcRepeatableDialogues[npcNextRepeatable]);
                    npcNextRepeatable = (npcNextRepeatable + 1) % npcRepeatableDialogues.Length;
                }
                break;
        }
    }
    #endregion

    #region Object Dialogue
    private void TriggerObjectDialogue(ObjectInteraction obj)
    {
        if (dialogueController != null && dialogueController.IsDialogueActive()) return;

        // One-time dialogue
        if (!obj.oneTimePlayedAll && obj.oneTimeDialogues != null && obj.nextOneTimeIndex < obj.oneTimeDialogues.Length)
        {
            PlayDialogue(obj.oneTimeDialogues[obj.nextOneTimeIndex]);
            obj.nextOneTimeIndex++;
            if (obj.nextOneTimeIndex >= obj.oneTimeDialogues.Length)
                obj.oneTimePlayedAll = true;
            return;
        }

        // Repeatable or Floating
        if (obj.repeatableDialogues != null && obj.repeatableDialogues.Length > 0)
        {
            PlayDialogue(obj.repeatableDialogues[obj.nextRepeatableIndex]);
            obj.nextRepeatableIndex = (obj.nextRepeatableIndex + 1) % obj.repeatableDialogues.Length;
        }
        else
        {
            bool forcePlayOnce = obj.postOneTimePriority == PostOneTimePriority.FloatingFirst;
            ShowFloatingDialogue(obj.floatingLines, ref obj.lastRandomFloating, ref obj.lastOrderedFloating, obj.playFloatingInOrder || forcePlayOnce, forcePlayOnce, ref obj.floatingPlayedAll);
        }
    }
    #endregion

    #region Dialogue Helpers
    private void ShowFloatingDialogue(string[] lines, ref int lastRandomIndex, ref int lastOrderedIndex, bool inOrder, bool playOnce, ref bool playedAll)
    {
        if (lines == null || lines.Length == 0) return;
        if (playOnce && playedAll) return;

        int index = 0;

        if (inOrder)
        {
            index = lastOrderedIndex + 1;
            if (index >= lines.Length)
            {
                if (playOnce)
                {
                    playedAll = true;
                    return;
                }
                index = 0;
            }
            lastOrderedIndex = index;
        }
        else
        {
            do { index = Random.Range(0, lines.Length); } while (index == lastRandomIndex && lines.Length > 1);
            lastRandomIndex = index;
        }

        if (floatingDialogueComponent != null)
            floatingDialogueComponent.ShowFloatingLine(lines[index]);
        else
            Debug.Log($"Floating: {lines[index]}");
    }

    private void PlayDialogue(Dialogue dialog)
    {
        if (dialog == null) return;
        if (dialogueController != null)
            dialogueController.StartDialogue(dialog);
        else if (dialog.lines != null)
        {
            foreach (var line in dialog.lines)
            {
                if (line == null) continue;
                string speaker = string.IsNullOrEmpty(line.speakerName) ? "???" : line.speakerName;
                Debug.Log($"{targetNPC.name} ({speaker}): {line.sentence}");
            }
        }
    }

    private DialogueController GetDialogueController()
    {
        if (dialogueController != null) return dialogueController;
        DialogueController found = FindFirstObjectByType<DialogueController>();
        if (found != null) dialogueController = found;
        return dialogueController;
    }
    #endregion
}