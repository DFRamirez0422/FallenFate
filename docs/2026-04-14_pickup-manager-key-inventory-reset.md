# David Glazier
## Key item inventory reset (no stale keys on reload / replay)
Date: April 14, 2026

## Change Descriptions

Key items (`Item_Data` with `pickupType == keys`) were staying in the pickup manager forever because `PickUp_Manager` used `DontDestroyOnLoad` and never cleared that list when the player returned to the main menu, restarted the same level, or continued after the game-over scene. World key pickups also destroy themselves on load if the key is already in the list, so keys could appear “permanently collected.” Key entries are now removed in those situations so keys and doors behave correctly on replay and level reload. Other pickup types (mementos, healing, etc.) are unchanged.

## Technical Explanations

- `PickUp_Manager` is now a singleton: the first instance calls `DontDestroyOnLoad` and subscribes to `SceneManager.sceneLoaded`; duplicate managers in newly loaded scenes are destroyed so only one inventory exists.
- On each loaded scene (after the first tracked transition), the script checks:
  - **Main menu** (`MainMenu`): clear all key items from `items`.
  - **Same scene as previous** (reload / retry same level): clear keys.
  - **Previous scene was game over** (`GameOver_NEW`): clear keys when loading the next scene (retry flow).
- Keys are removed with `List.RemoveAll` where `pickupType == Item_Data.PickUpType.keys`. Clearing keys allows `PickUpObjects` to keep scene key prefabs when appropriate (they are only destroyed if the key is still in the list).
- The first scene after a cold start only seeds `s_PreviousSceneName` so we do not treat the initial load as a “reload.”

## File Location Tracking

No files were moved between branches.

## Communication Context

Angel Rodriguez owns `PickUp_Manager` and pickup / door flows; anyone relying on keys persisting across a **same-scene reload** or **full replay** should know keys now reset in those cases (by design). Cross-act progression that only changes scene name still keeps keys until one of the reset conditions above applies.

# File Locations

| File / Asset | Path |
|--------------|------|
| PickUp_Manager | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/PickUp_Manager.cs` |
