# David Glazier
## Jose's Dialogue System Fixes
Date: February 24, 2026

## Change Descriptions
- Fixed dialogue progression so one key press advances exactly one line, preventing skipped or jumbled text.
- Fixed NPC/trigger conversation selection so only one valid conversation node is consumed at a time instead of removing all valid nodes in a single pass.
- Added guards to prevent null or empty dialogue assets from starting a conversation and silently breaking dialogue flow.
- Added safer UI listener cleanup to avoid accidental action button listener stacking between dialogue states.

## Technical Explanations
- `DialogueManager` now owns `Interact` progression input while dialogue is active. This removes duplicate `AdvanceDialogue()` calls from `NPCTalk` and `DialogueTrigger`, which previously caused race-like double advancement.
- `StartDialogue()` now exits early if the incoming `DialogueSO` is null or has no lines. `AdvanceDialogue()` and `ShowDialogue()` also guard against invalid state.
- Choice rendering now uses `Mathf.Min(options, buttons)` to avoid out-of-range button access when option count exceeds available UI buttons.
- `CheckForNewConversation()` in both talk/trigger scripts now selects and removes only the first valid conversation in list order, then returns immediately. This preserves progression order and prevents unintended looping/jump behavior.
- `DialogueTrigger` no longer advances dialogue on `Interact`; it only starts dialogue on player trigger entry and waits for `DialogueManager` to drive progression.

## Exact Code Changes (Brief)
- `DialogueManager.cs`
  - Added:
    - `if (IsDialogueActive && Input.GetButtonDown("Interact")) { AdvanceDialogue(); }`
    - Why: ensures only one central script advances dialogue per key press.
  - Changed:
    - `Time.timeScale = 0.01f;` -> `Time.timeScale = 0.0f;`
    - Why: fully pauses gameplay during dialogue instead of slow-motion updates.
  - Added guards:
    - `if (dialogueSO == null || dialogueSO.lines == null || dialogueSO.lines.Length == 0) return;`
    - `if (!IsDialogueActive || m_CurrentDialogue == null || m_CurrentDialogue.lines == null) return;`
    - Why: prevents invalid dialogue states from starting/advancing.
  - Changed:
    - `int choiceCount = Mathf.Min(m_CurrentDialogue.options.Length, m_ChoiceButtons.Length);`
    - Why: avoids button index overflow when options exceed UI button count.
  - Changed cleanup:
    - `m_ActionButton.onClick.RemoveAllListeners();` in `ClearChoices()`
    - Why: prevents stale listener carry-over between dialogue states.

- `NPCTalk.cs`
  - Removed advancing path while dialogue is active (`DialogueManager.Instance.AdvanceDialogue();`).
  - Added early return:
    - `if (!Input.GetButtonDown("Interact") || DialogueManager.Instance.IsDialogueActive) return;`
  - Changed conversation selection loop to consume one valid entry then `return`.
  - Why: prevents double-advance and preserves intended conversation order.

- `DialogueTrigger.cs`
  - Removed `Update()` interaction advancement block that called `AdvanceDialogue()`.
  - Changed trigger guard:
    - `if (collision.gameObject.CompareTag("Player") && !DialogueManager.Instance.IsDialogueActive)`
  - Added null start guard:
    - `if (m_CurrentConversation != null) { DialogueManager.Instance.StartDialogue(m_CurrentConversation); }`
  - Changed conversation selection loop to consume one valid entry then `return`.
  - Why: prevents concurrent dialogue control and unintended looping/jump behavior.

## File Location Tracking
- No files were moved between branches for this change set.

## Communication Context
- **Team member to inform:** Jose E.
- This change directly affects conversation progression behavior used by Jose's dialogue SO workflow and trigger/NPC interaction pattern.
- Jose should verify his prefab/scene setups still point to intended `DialogueSO` assets and that per-trigger conversation list order matches desired progression.

# File Locations
| File / Asset | Path |
|--------------|------|
| DialogueManager | `Assets/Scripts/NPC/DialogueManager.cs` |
| NPCTalk | `Assets/Scripts/NPC/NPCTalk.cs` |
| DialogueTrigger | `Assets/Scripts/NPC/DialogueTrigger.cs` |
| Documentation | `docs/2026-02-24_joses-dialogue-system-fixes.md` |
