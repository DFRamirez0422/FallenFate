# David Glazier
## Act 2 & Act 3 story beat DialogueSOs (Dante monologue)
Date: April 14, 2026

## Change descriptions
Added nine single-line `DialogueSO` assets from `All Story Dialogue Boxes.txt`: Level 2 (four lines) and Level 3 (five lines). Each numbered beat is its own asset. Speaker is **Dante** only (`ActorSO` guid `7ce02c1f8c657064e91da607aebe315a`). Act 3 filenames include the script section labels (Solitary, Nightmare, Ruins, Rooftop) for layout reference.

## Technical explanations
- Each asset has **one** `DialogueLine` (one continue press in typical UI).
- **Emotion** uses `ActorSO.Emotion` integers: `1` = Idle_Talk, `2` = Sad, `3` = Sad_Talk, `5` = Quizzical_Talk.
- **Level 2:** uneasy / reflective lines use Quizzical_Talk (5) or Idle_Talk (1); heavier regret uses Sad (2); hopeful closing line uses Sad_Talk (3).
- **Level 3:** disorientation and realization beats use Quizzical_Talk (5); somber and acceptance beats use Sad (2) or Sad_Talk (3) on the final rooftop line.

## File location tracking
No files moved between branches.

## Communication context
Narrative / level scripting: wire triggers or `nextDialogue` chains in order `01→02→…` per act as needed.

# File locations
| File / Asset | Path |
|--------------|------|
| Act2 story L2 #1 | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Act2_Story_L2_01.asset` |
| Act2 story L2 #2 | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Act2_Story_L2_02.asset` |
| Act2 story L2 #3 | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Act2_Story_L2_03.asset` |
| Act2 story L2 #4 | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Act2_Story_L2_04.asset` |
| Act3 story L3 #1 Solitary | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_Story_L3_01_Solitary.asset` |
| Act3 story L3 #2 Solitary | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_Story_L3_02_Solitary.asset` |
| Act3 story L3 #3 Nightmare | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_Story_L3_03_Nightmare.asset` |
| Act3 story L3 #4 Ruins | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_Story_L3_04_Ruins.asset` |
| Act3 story L3 #5 Rooftop | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Act3_Story_L3_05_Rooftop.asset` |
