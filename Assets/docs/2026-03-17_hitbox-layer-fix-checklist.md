# David Glazier
## Hitbox Layer Fix — Checklist for Nathan
Date: March 17, 2026

---

Okay got it working! But I don't know why it wasn't set to default on your end. Double check the prefabs and every scene for this:

- **Hitbox Tag** — Make sure any player Hitbox child object has its tag set to `"Hitboxs"`, not `"Untagged"`. Your scripts (`AngryEchoHitbox`, `WardenHitbox`, `GrabberHitBox`) all check for `collision.gameObject.tag == "Hitboxs"` — if the tag isn't set, the hit never registers.
- **Hitbox Collider is Trigger** — The player Hitbox child's CapsuleCollider2D needs `Is Trigger` checked. It's only for combat detection, not physics blocking. If it's not a trigger, it will double-collide with environment objects.
- **Physics 2D Collision Matrix** — In Project Settings > Physics 2D, the **Hitboxs** layer must be enabled to collide with the **Enemy** layer. If that box is unchecked, enemy triggers will never fire against the player hitbox.
- **Enemy Layer Masks** — On every enemy prefab using `EnemyCombat` or `EnemyMovement`, the **Player Layer** mask field in the Inspector needs to include the **Hitboxs** layer, not just `Player`. The `OverlapCircleAll` calls use this mask to find the player — if it only checks the `Player` layer, it won't find the Hitbox child on the `Hitboxs` layer.
- **Scene Overrides** — Check each scene for prefab overrides that might revert any of the above (tag, trigger, layer mask). Unity scene instances can override prefab values, so a fix to the prefab won't apply if a scene instance has its own saved value.

---

## Code Changes Made

| Change | File |
|--------|------|
| `GetComponent` → `GetComponentInParent` for `PlayerHealth` and `PlayerMovement` | `Assets/Scripts/Enemy/EnemyCombat.cs` |
| `GetComponent` → `GetComponentInParent` for `PlayerMovement`, `hits[0].transform` → `hits[0].transform.root` | `Assets/Scripts/Enemy/EnemyMovement.cs` |
| Hitbox child tag `"Untagged"` → `"Hitboxs"`, collider `IsTrigger` → `true` | `Assets/PreFabs/Player/Player.prefab` |

## File Locations
| File / Asset | Path |
|--------------|------|
| EnemyCombat | `Assets/Scripts/Enemy/EnemyCombat.cs` |
| EnemyMovement | `Assets/Scripts/Enemy/EnemyMovement.cs` |
| Player Prefab | `Assets/PreFabs/Player/Player.prefab` |
| AngryEchoHitbox | `Assets/Sandbox/Nathan White/Nathan's Scripts/Echo Angry Scripts/AngryEchoHitbox.cs` |
| WardenHitbox | `Assets/Sandbox/Nathan White/Nathan's Scripts/Warden Scripts/WardenHitbox.cs` |
| GrabberHitBox | `Assets/Sandbox/Nathan White/Nathan's Scripts/Grabber Scripts/GrabberHitBox.cs` |
| Physics 2D Settings | `ProjectSettings/Physics2DSettings.asset` |
