# David Glazier
## Act 1 Cutscene, Dialogue, and Tools Update
Date: February 24, 2026

## Change Descriptions
- Added and integrated Act 1 cutscene/scene content updates across multiple main scene files.
- Stabilized dialogue progression logic to prevent skipped/jumbled lines and unintended loop behavior.
- Fixed player build compilation by removing an editor-only namespace import from runtime player code.
- Added reusable sandbox utility scripts for trigger-based object spawn/despawn and scene-load countdown events.
- Added a no-player-reference scene transition script for cutscene flow usage.

## Technical Explanations
- Dialogue input advancement is now centralized in `DialogueManager` while active dialogue is open, reducing duplicate advancement calls from multiple scripts.
- Dialogue conversation selection in NPC/trigger scripts now consumes one valid conversation at a time and preserves list order.
- Runtime guard clauses were added to dialogue flow methods to prevent null/empty dialogue nodes from entering invalid states.
- Countdown utility supports scene-start countdown and invokes an inspector-assigned `UnityEvent` on completion.
- Scene transition utility for cutscenes handles fade animation and delayed scene load without requiring a player object reference.

## File Location Tracking
- No files were moved between branches in this update.

## Communication Context
- **Team members to inform:** Jose E. (dialogue flow behavior), anyone editing Act 1/2/3 scene content, and anyone maintaining render/pipeline settings.
- Dialogue behavior changes may affect prefabs/scenes that previously relied on trigger-side input advancement.
- Scene and pipeline settings changes should be validated in a clean pull on another machine to confirm expected rendering/build behavior.

# File Locations
| File / Asset | Path |
|--------------|------|
| Act 1 Main Scene | `Assets/Main Scenes/Act 1 ferry forest/Act 1 REMAKE.unity` |
| Act 2 Main Scene | `Assets/Main Scenes/Act 2 cellblocks/Act 2 Main Scene.unity` |
| Act 3 Solitary Scene | `Assets/Main Scenes/Act 3 solidary confinement/Scene 1 Solitary.unity` |
| Act 3 Nightmare Scene | `Assets/Main Scenes/Act 3 solidary confinement/Scene 3 Nightmare.unity` |
| Act 1 Cutscene Scene | `Assets/Main Scenes/Act 1 ferry forest/Act 1 CUTSCENE.unity` |
| Dialogue Manager | `Assets/Scripts/NPC/DialogueManager.cs` |
| Dialogue Trigger | `Assets/Scripts/NPC/DialogueTrigger.cs` |
| NPC Talk | `Assets/Scripts/NPC/NPCTalk.cs` |
| Player Animator | `Assets/Scripts/Player/PlayerAnimator.cs` |
| Cutscene Scene Changer | `Assets/Scripts/Core/sceneChangeNoPlayerObj.cs` |
| Trigger Object Spawner | `Assets/Sandbox/David_G/TriggerObjectSpawner.cs` |
| Scene Load Countdown Timer | `Assets/Sandbox/David_G/SceneLoadCountdownTimer.cs` |
| Fader Canvas | `Assets/Sandbox/David_G/FaderCanvas.prefab` |
| Act1 Dialogue Folder Meta | `Assets/Sandbox/David_G/Dialouge Objects/Act1.meta` |
| Act1 Dialogue Meta 01 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_01_Docks_Intro.asset.meta` |
| Act1 Dialogue Meta 02 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_02_Docks_FerryRamp.asset.meta` |
| Act1 Dialogue Meta 03 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_03_WoodPath_WrongWay.asset.meta` |
| Act1 Dialogue Meta 04 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_04_PrisonEntrance_Locked.asset.meta` |
| Act1 Dialogue Meta 05 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_05_WoodsPath.asset.meta` |
| Act1 Dialogue Meta 06 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_06_Oceanfront_Dorothy.asset.meta` |
| Act1 Dialogue Meta 07 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_07_WoodsPath_AfterDorothy.asset.meta` |
| Act1 Dialogue Meta 08 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_08_SecurityShed_Key.asset.meta` |
| Act1 Dialogue Meta 09 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_09_KeyAcquired.asset.meta` |
| Act1 Dialogue Meta 10 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_10_Memento.asset.meta` |
| Act1 Dialogue Meta 11 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_11_Bench_Empty.asset.meta` |
| Act1 Dialogue Meta 12 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_12_Echo_Attack.asset.meta` |
| Act1 Dialogue Meta 13 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_13_PrisonEntrance_Unlock.asset.meta` |
| Act1 Dialogue Meta 14 | `Assets/Sandbox/David_G/Dialouge Objects/Act1/Act1_14_Gate_Shuts.asset.meta` |
| Cutscenes Folder | `Assets/Animations/CutScenes/` |
| Cutscenes Folder Meta | `Assets/Animations/CutScenes.meta` |
| Performance Test Info | `Assets/Resources/PerformanceTestRunInfo.json` |
| Performance Test Info Meta | `Assets/Resources/PerformanceTestRunInfo.json.meta` |
| Performance Test Settings | `Assets/Resources/PerformanceTestRunSettings.json` |
| Performance Test Settings Meta | `Assets/Resources/PerformanceTestRunSettings.json.meta` |
| PC Pipeline Asset | `Assets/Settings/PC_PipelineAsset.asset` |
| TMP Fallback Font Asset | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset` |
| URP Global Settings | `Assets/UniversalRenderPipelineGlobalSettings.asset` |
| Build Settings | `ProjectSettings/EditorBuildSettings.asset` |
| Graphics Settings | `ProjectSettings/GraphicsSettings.asset` |
| Existing Dialogue Fix Doc | `docs/2026-02-24_joses-dialogue-system-fixes.md` |
| Docs Index | `docs/README.md` |
