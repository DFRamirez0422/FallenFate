# David Glazier
## Act1 After Guitar key pickup trigger chain reduction
Date: April 21, 2026

## Change descriptions
In `Act 1 after_guitar.unity`, the `Key_GateKey` pickup prefab instance had a `TriggerSpawner` list with three follow-up toggles. Two of those toggles activated nested `Act1_09_KeyAcquired` dialogue trigger objects immediately after key collection.

Those two key-acquired trigger activations were removed from the key's toggle list. The list now keeps only one entry, preserving the existing Dorothy despawn toggle.

## Technical explanation
- The key object is a `Key_GateKey` prefab instance (`guid: 7b102992694a0d541988992093a98df2`) with a `TriggerSpawner` (`objectsToToggle`).
- Before: `objectsToToggle.Array.size = 3` with targets:
  - `Act1_09_KeyAcquired`
  - `Act1_09_KeyAcquired (1)`
  - Dorothy object (despawn action)
- After: `objectsToToggle.Array.size = 1`, only:
  - Dorothy object with `action = Despawn`.

By removing the two key-acquired trigger objects from the key pickup chain, the post-key activation path that was kicking off the camera waypoint sequence no longer runs from key collection.

## File Locations
| File / Asset | Path |
|--------------|------|
| Act 1 after guitar scene | `Assets/-Main Scenes/Act 1/Act1AfterGuitar/Act 1 after_guitar.unity` |

## Communication context
If anyone owns the Act 1 dialogue/camera flow, they should be aware that key pickup no longer enables the two `Act1_09_KeyAcquired` trigger objects. If you still want the dialogue without the camera movement, that should be re-wired via a separate trigger or conversation setup.
