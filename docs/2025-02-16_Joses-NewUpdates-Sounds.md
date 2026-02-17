# Jose Escobedo
## [PR #428] New Updates including Sounds – Merge Documentation
Date: February 16, 2025

---

## Change Descriptions

PR #428 introduced player animations (Hurt, Death), a centralized sound system (`PlayerSound`), camera auto-tracking across scenes, and robustness fixes for scene transitions. The merge combined these changes with main-branch combat feedback (PR #417) and resolved conflicts in `PlayerCombat.cs` and `Player.controller`.

### What Was Merged

- **Player animations:** Hurt (4 directions) and Death (4 directions) clips and controller states
- **PlayerSound:** Centralized audio for attack, damage, footsteps, collision, wall hit
- **CameraManager:** Auto-locates player on scene load for Cinemachine
- **SceneChange:** Null-safe animator check so missing animator no longer locks the game
- **GameOverScreen / NPCTalk:** Sound support hooks
- **Audio assets:** Game Over, attack slash, taking damage (footsteps removed per PR notes – 53s clip)
- **Prefabs:** Respawn_Point, Scene_Change

### What Was Combined or Removed During Merge

| Area | Main (HEAD) | PR #428 | Resolution |
|------|-------------|---------|------------|
| **PlayerCombat – Attack sound** | `m_PlayerAudio.PlayOneShot(m_AttackSwingClip)` + `m_AttackVoiceClip` | `m_PlayerSound.PlayAttack()` | **Combined:** Prefer `PlayerSound.PlayAttack()` when present; fallback to `m_PlayerAudio` + `m_AttackSwingClip` |
| **PlayerCombat – Hurt sound** | `m_PlayerAudio.PlayOneShot(m_HurtClip)` | `PlayerSound.PlayDamage()` | **Combined:** Prefer `PlayerSound.PlayDamage()`; fallback to `m_PlayerAudio` + `m_HurtClip` |
| **PlayerCombat – Attack flow** | Direct animator play + AudioSource | `m_PlayerAnimator.StartAttack()` + `PlayerSound` | **Combined:** Use `StartAttack()` for animation state; use `PlayerSound` or AudioSource for sound |
| **PlayerCombat – Serialized fields** | `m_PlayerAudio`, `m_AttackSwingClip`, `m_HurtClip`, `m_AttackVoiceClip` | None (PlayerSound only) | **Kept:** AudioSource fields retained as optional fallback; `m_AttackVoiceClip` removed |
| **Player.controller** | HurtLeft, HitDirY transitions, different structure | Hurt_Left, Hurt_Forward, etc., flat structure | **Used PR version:** Matches `PlayerAnimator` state names (Hurt_Left, Hurt_Forward, etc.) |

### Removed or Not Merged

- **m_AttackVoiceClip:** Dropped from `PlayerCombat`; use `PlayerSound` or a single attack clip
- **Footstep audio:** PR notes say footsteps were removed to avoid playing 53 seconds of audio; floor-type logic exists in `PlayerSound` but is not wired (TODO)
- **Player copy 5/6 controllers:** Sandbox animator copies in JoseE folder; kept for reference, not used by main Player

---

## Technical Explanations

### PlayerSound

- **Location:** `Assets/Scripts/Player/PlayerSound.cs`
- **Usage:** Add to Player prefab; assign clips in Inspector.
- **Methods:** `PlayAttack()`, `PlayDamage()`, `PlayFootstep()`, `PlayCollision()`, `PlayWallHit()`
- **FloorType:** Enum for Normal, Snow, Stairs, Bush; currently defaults to Normal. Floor map integration is TODO.

### PlayerAnimator

- **State flow:** `StartAttack()`, `StartDamage()`, `StartDeath()` set internal state; `Update()` drives `m_Animator.Play()` based on `LastMovedDirection` and state.
- **State names:** Idle_Left/Right/Back/Forward, Walk_*, Attack_*, Hurt_*, Death_*

### CameraManager

- **Usage:** Add to Cinemachine camera; on `sceneLoaded`, sets `Target.TrackingTarget` to `GameObject.FindGameObjectWithTag("Player")`.
- **Requirement:** Player must have "Player" tag in loaded scene.

### SceneChange

- **Change:** `if (m_FadeScreenAnimator)` before `Play("FadeOut")` – prevents NullReferenceException if animator is unassigned.
- **Note:** Still requires animator for fade; missing animator simply skips fade, does not lock game.

---

## File Locations

| File / Asset | Path |
|--------------|------|
| PlayerSound | `Assets/Scripts/Player/PlayerSound.cs` |
| PlayerCombat | `Assets/Scripts/Player/PlayerCombat.cs` |
| PlayerAnimator | `Assets/Scripts/Player/PlayerAnimator.cs` |
| PlayerMovement | `Assets/Scripts/Player/PlayerMovement.cs` |
| PlayerHealth | `Assets/Scripts/Player/PlayerHealth.cs` |
| CameraManager | `Assets/Scripts/Core/CameraManager.cs` |
| SceneChange | `Assets/Scripts/Core/SceneChange.cs` |
| GameOverScreen | `Assets/Scripts/Core/GameOverScreen.cs` |
| NPCTalk | `Assets/Scripts/NPC/NPCTalk.cs` |
| Player.controller | `Assets/Animations/Dante/Player.controller` |
| PlayerDeath_* | `Assets/Animations/Dante/PlayerDeath_Back.anim`, etc. |
| PlayerHurt_* | `Assets/Animations/Dante/PlayerHurt_Back.anim`, etc. |
| Respawn_Point | `Assets/PreFabs/Player/Respawn_Point.prefab` |
| Scene_Change | `Assets/PreFabs/Player/Scene_Change.prefab` |
| GameOverScreen prefab | `Assets/PreFabs/UI/GameOverScreen.prefab` |
| Game over sound | `Assets/audio/GameOver/Game over sound effect.wav` |
| Attack slash | `Assets/audio/Player/Attck slash.wav` |
| Taking damage | `Assets/audio/Player/Taking damage.wav` |

---

## Communication Context

- **Author:** Jose Escobedo (@JoseEscobedo02)
- **Team members to inform:** Anyone using Player prefab (ensure `PlayerSound` is added and clips assigned); level designers using SceneChange (animator optional but recommended)
- **Known issues (from PR):** Sound effects may not play correctly if not cut properly; clip names may not match descriptions
