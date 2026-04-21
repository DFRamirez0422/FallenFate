# David Glazier
## Guitar battle transition prefabs + dialogue end instantiation wiring
Date: April 21, 2026

## Change descriptions
Created missing guitar transition prefabs for the Act 2 and Act 3 guitar battles and aligned all four guitar transition prefabs to use the same scene transition material. Then wired the guitar challenger dialogue assets so dialogue end now instantiates the correct transition prefab for each battle.

## Technical explanation
- Transition prefabs use `SceneLoadCountdownTimer` (`m_StartTimeSeconds: 1.5`) and invoke `DG_scene_change.LoadScene` on timer completion.
- Added new prefabs:
  - `GuitarTransition_exitDoor` -> loads `GuitarBattle_Exit Door`
  - `GuitarTransition_rooftop` -> loads `GuitarBattle_Rooftop`
- Existing prefabs retained for Act 1:
  - `GuitarTransition_tutorialGuitar` -> loads `GuitarBattle_Tutorial Duel`
  - `GuitarTransition_aftershed` -> loads `GuitarBattle_after_shed`
- Standardized transition image material across all four prefabs to `SceneTransitionMaterial` (`guid: 34629c11ec404cd40966f519d6295931`).
- Dialogue SO wiring:
  - `Guitar_TRUTH _challenger.asset` and `Guitar_ACCEPTANCE _challenger.asset` changed to `actionOnDialogueEnd: 4` and now reference the new transition prefabs in `objectsToInstantiate`.
  - `Guitar_GUILT _challenger.asset` and `Guitar_AVOIDANCE_challenger1.asset` already referenced transition prefabs and were verified.

## File Locations
| File / Asset | Path |
|--------------|------|
| Tutorial transition prefab | `Assets/Sandbox/David_G/DG_RythmTests/transition_guitar/GuitarTransition_tutorialGuitar.prefab` |
| After shed transition prefab | `Assets/Sandbox/David_G/DG_RythmTests/transition_guitar/GuitarTransition_aftershed.prefab` |
| Exit door transition prefab (new) | `Assets/Sandbox/David_G/DG_RythmTests/transition_guitar/GuitarTransition_exitDoor.prefab` |
| Rooftop transition prefab (new) | `Assets/Sandbox/David_G/DG_RythmTests/transition_guitar/GuitarTransition_rooftop.prefab` |
| Transition material | `Assets/Sandbox/Luis E/OLD Version/Luis Espinoza/Images/SceneTransitionMaterial.mat` |
| Act1 Avoidance challenger dialogue | `Assets/DialogueSOs/DialogueText/Act1_DialogueSOs/Guitar_AVOIDANCE_challenger1.asset` |
| Act1 Guilt challenger dialogue | `Assets/DialogueSOs/DialogueText/Act1_DialogueSOs/Guitar_GUILT _challenger.asset` |
| Act2 Truth challenger dialogue | `Assets/DialogueSOs/DialogueText/Act2_DialogueSOs/Guitar_TRUTH _challenger.asset` |
| Act3 Acceptance challenger dialogue | `Assets/DialogueSOs/DialogueText/Act3_DialogueSOs/Guitar_ACCEPTANCE _challenger.asset` |

## Communication context
Anyone maintaining guitar challenger dialogue flow should know that Act2/Act3 now transition via instantiated transition prefabs on dialogue end (instead of relying only on direct scene action options). If dialogue-end action behavior changes in the dialogue system, these four SOs should be revalidated together.
