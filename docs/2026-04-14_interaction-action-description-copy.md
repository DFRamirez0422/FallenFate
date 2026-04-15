# David Glazier
## Interaction prompts (ActionDescription copy)
Date: April 14, 2026

## Change descriptions
World interaction UI text was aligned with `Interaction Dialogues TO FIX.txt` for Level 2 (powered exit door, generators, Warden office door). Optional per-object strings were added so other keyed doors keep the generic locked message unless configured.

## Technical explanations
- **`Powered_Door`:** Replaced the old “not powered” line with state-specific copy: no generators, one generator (`Door Power 50%`), or both generators ready. When both are on, **“Both Generators Activated”** is combined with **“Open Door”** on the main Description text (newline) so it is not placed in the red **Unlock_NotMeet** slot, which overlaps the primary prompt. Added `OnCollisionStay2D` so the prompt updates if a generator comes online while the player stands at the door. Fixed exit cleanup to destroy the spawned instance instead of toggling the prefab asset.
- **`OpenDoors`:** Added optional `lockedStatusText` and `unlockedStatusText`. `WardenOffice_Door` prefab sets these to the Warden’s Office LOCKED / UNLOCKED lines. Other doors leave them empty and still use `locked. Find the key.` when locked. Exit handler now only destroys the spawned prompt instance. When the player has the key and `unlockedStatusText` is set, that string goes on the main **Description** text and the red **Unlock_NotMeet** slot is left empty so it does not overlap “Open Door” / the interact hint.
- **`Activate_Generators`:** Added optional `interactPromptWhenOff` and `statusPromptWhenOn`. `Generator.prefab` defaults to Library OFF / ACTIVATED; `Testing.unity` overrides the second instance to Warden’s Office OFF / ACTIVATED.

## File location tracking
No paths moved between branches.

## Communication context
Level designers using duplicate generator instances should set Warden vs Library overrides on each instance if not using `Testing.unity`.

# File locations
| File / Asset | Path |
|--------------|------|
| OpenDoors | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/OpenDoors.cs` |
| Powered_Door | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/Powered_Door.cs` |
| Activate_Generators | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/Activate_Generators.cs` |
| Warden office door prefab | `Assets/Sandbox/Angel_Rodriguez/Resources/Prefabs/Room2_Doors/WardenOffice_Door.prefab` |
| Generator prefab | `Assets/Sandbox/Angel_Rodriguez/Resources/Prefabs/Room2_Doors/Generator.prefab` |
| Testing scene overrides | `Assets/Sandbox/Angel_Rodriguez/Resources/Testing.unity` |
| Reference doc (not in repo) | `c:\Users\dglaz\Downloads\Interaction Dialogues TO FIX.txt` |
