# David Glazier
## NPC dialogue — final script and emotion alignment
Date: April 14, 2026

## Change descriptions
Dialogue ScriptableObjects for Dorothy (Act 1), Mira (Act 2), and David (Act 3) were updated so line text matches the approved master document `FINAL--All NPCs Dialogues.txt` verbatim (Unicode ellipsis, apostrophes, and em dash where specified). Line emotions were set from the parenthetical cues in that file, mapped to `ActorSO.Emotion` values used by `DialogueSO`.

## Technical explanations
- **Text:** Mira’s node had a truncated sixth line and `…Elena?` used ASCII periods instead of U+2026. David’s fifth line was missing the `Beautiful?…She's—…` beat from the final script.
- **Emotions:** Parentheticals map as follows: **Neutral** → `Idle_Talk` (1); **Confused** → `Quizzical_Talk` (5); **Sad** → `Sad` (2); **Angry** → `Angry` (6). Dorothy’s sad lines were shifted from `Sad_Talk` (3) to `Sad` (2) to match the `(Sad)` label.
- **David Angry:** `David.asset` currently defines emotion portraits only through `Quizzical_Talk` (5). Line 6 uses `Angry` (6); until an angry portrait is added to that actor, `DialogueManager` falls back to `m_DefaultPortrait` for that line.

## File location tracking
No files were moved between branches.

## Communication context
Anyone owning narrative, VO, or portrait art should know about the David angry line portrait gap and the Mira/David text corrections.

# File locations
| File / Asset | Path |
|--------------|------|
| Act1 Dorothy dialogue | `Assets/DialogueSOs/DialogueText/Act1_DialogueSOs/Act1_Dorothy_Dialogue.asset` |
| Act2 Mira first encounter | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Act2_01_Mira_FirstEncounter.asset` |
| Act3 David first encounter | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_01_David_FirstEncounter.asset` |
| Master script (reference, not in repo) | `c:\Users\dglaz\Downloads\FINAL--All NPCs Dialogues.txt` |
