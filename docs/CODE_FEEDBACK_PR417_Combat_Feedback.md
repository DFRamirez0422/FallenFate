# Code feedback: Erik and Mike Combat feedback first pass (PR #417) vs current main

**Compared:** `pr-417` (PR #417 head) vs `main`  
**Scope:** Combat-related scripts, EnemyHealth, PlayerHealth, PlayerAnimator, new ErikA scripts

---

## Summary

PR #417 adds 4-direction attack animations, hit reactions (flash, sound, impact VFX), camera shake, and splits healing vs damage in health. Several issues should be fixed before or right after merge: **EnemyHealth** can throw if `EnemyHitScript` is missing, **PlayerHealth** loses game-over and death flow, **PlayerCombat** no longer moves the attack point with the player, and **CameraShake** / **EnemyHitScript** have small bugs and cleanup items.

---

## 1. PlayerCombat.cs

### PR 417 vs main

| Aspect | Main | PR 417 |
|--------|------|--------|
| Attack direction | Single “IsAttacking” bool | 4-direction: `AttackUp/Down/Left/Right` via `PlayerAnimator.LastMovedDirection` |
| Attack point | `MoveAttackPoint()` in Update using `PlayerMovement.CurrentDirection` | No `MoveAttackPoint()`; no `m_PlayerMovement` |
| Audio | None | `m_PlayerAudio.PlayOneShot(m_AttackSwing)` in `Attack()` |
| Dependencies | `PlayerMovement`, `Animator` | `PlayerAnimator`, `Animator` |

### Feedback

- **Critical – attack point:** Main moves `m_AttackPoint` every frame with `MoveAttackPoint()` so the hitbox follows facing. PR 417 removes this. If `m_AttackPoint` is still used in `DealDamage()`, it will not follow the player’s direction and hits will be wrong. **Either re-add `MoveAttackPoint()` (and a reference to movement/direction)** or document that attack point is set another way (e.g. by animation).
- **Good:** Direction-based attack states, Tooltips, optional input flag, cooldown and gizmo unchanged.
- **Naming:** Consider `m_AttackSwingClip` (or similar) so it’s clear it’s a clip, not a swing state.
- **Animation:** `m_Animator.Play(state)` is correct for non-blend-tree states; ensure controller has matching state names or fallbacks.

---

## 2. EnemyHealth.cs

### PR 417 vs main

- PR 417 adds: `UnityEvent m_OnHit`, `EnemyHitScript m_HitReaction`, and on hit calls `m_OnHit?.Invoke()`, `m_HitReaction.FlashWhite()`, `PlayHitSound()`, `ImpactEffect()`.
- Init moved from `Start()` to `Awake()` and `m_HitReaction = GetComponent<EnemyHitScript>()`.

### Feedback

- **Critical – null reference:** If an enemy has `EnemyHealth` but no `EnemyHitScript`, `m_HitReaction` is null and `m_HitReaction.FlashWhite()` (and the other calls) will throw. **Use null checks before calling:**
  ```csharp
  m_OnHit?.Invoke();
  m_HitReaction?.FlashWhite();
  m_HitReaction?.PlayHitSound();
  m_HitReaction?.ImpactEffect();
  ```
- **Typo:** “hit pints” → “hit points” in Tooltip.
- **Unused:** `using Unity.Mathematics` – remove if not used.
- **Good:** UnityEvent for hit, separation of hit reaction into `EnemyHitScript`.

---

## 3. PlayerAnimator.cs

### PR 417 vs main

- PR 417 adds: `m_LastMovedDirection`, `LastMovedDirection` property, and `QuantizeTo8(direction)` so direction is stored and exposed for combat.
- `SetCurrentDirection` now updates `m_LastMovedDirection` (quantized) and no longer sets animator floats `LastDirX` / `LastDirY` (that logic is commented/removed).

### Feedback

- **Compatibility:** Main’s animator may rely on `LastDirX` / `LastDirY` for movement/blend trees. If PR 417 is merged, ensure the controller and any other scripts don’t depend on those floats, or set them in addition to `m_LastMovedDirection`.
- **Redundancy:** Both `Start()` and `Awake()` assign `m_Animator`; one is enough (e.g. `Awake()`).
- **Good:** `QuantizeTo8` for 8-direction is clear; exposing `LastMovedDirection` is useful for combat.

---

## 4. PlayerHealth.cs

### PR 417 vs main

| Aspect | Main | PR 417 |
|--------|------|--------|
| Events | `m_OnZeroHealth` | `m_OnHeal`, `m_OnHit`, `m_OnZeroHealth` |
| ChangeHealth | `m_CurrentHealth += amount`; clamp; if ≤0 invoke `m_OnZeroHealth` | Routes to `Heal(amount)` or `Hit(-amount)`; no clamp in one path |
| Zero health | `m_OnZeroHealth?.Invoke()` (then typically `StartPlayerDeath()` from event) | `gameObject.SetActive(false)` only |
| Removed in PR 417 | — | `m_GameOverScreenPrefab`, `StartPlayerDeath()`, `ResetHealth()` |

### Feedback

- **Critical – game over / death:** Main uses `m_OnZeroHealth` so something (e.g. GameOverScreen) can call `StartPlayerDeath()` (disable movement, play death anim, show game over). PR 417 removes `StartPlayerDeath()` and `ResetHealth()` and only does `SetActive(false)`. That will break the current death/game-over flow. **Either keep `StartPlayerDeath()` and `ResetHealth()` and call them from the existing `m_OnZeroHealth` flow, or** introduce an equivalent path (e.g. event that triggers death UI and reset).
- **Critical – clamping:** In PR 417, `Hit()` only subtracts and invokes events; it doesn’t clamp `m_CurrentHealth` to a minimum of 0. If something applies damage twice or in the same frame, health could go negative. Consider `m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - amount)` and then check for zero.
- **Typos:** “variaable” → “variable”, “hit pints” → “hit points”, “channge” → “change”.
- **Good:** Separate Heal/Hit and extra events are useful for UI and feedback.

---

## 5. CameraShake.cs (new in PR 417)

### Feedback

- **Remove `Debug.Log("Shake")`** – avoid log spam in production.
- **Empty `Update()`** – remove it.
- **2D vs 3D:** `Random.insideUnitSphere` is 3D; for a 2D game you may want to keep the camera’s Z. For example:
  ```csharp
  Vector2 offset = Random.insideUnitCircle * shakeAmount;
  transform.position = initialPos + new Vector3(offset.x, offset.y, 0f);
  ```
- **Single-frame shake:** `PlayShake()` sets position once; often camera shake is applied over several frames (e.g. in Update for a short duration). Consider a small timer and decaying offset so the shake is visible and smooth.
- **Naming:** `Awake`/`Update` are normally private in Unity; consider making them private for consistency.

---

## 6. DisableHitEffect.cs (new in PR 417)

- Simple and fine. Optionally add a short Tooltip that it’s intended to be called from an animation event (if that’s the use case).

---

## 7. EnemyHitScript.cs (new in PR 417)

### Feedback

- **Unused using:** Remove `using Unity.VisualScripting;` if not used.
- **Null safety:** In `Awake()`, `m_EnemySprite`, `m_FlashMaterial`, or `m_ImpactEffect` can be null (e.g. not set in Inspector). `m_EnemySprite.material` and `m_ImpactEffect.GetComponent<Animator>()` can throw. Add null checks and early-outs (or safe defaults) in `FlashWhite()`, `ImpactEffect()`, and Awake.
- **ImpactEffect():**
  - Redundant check: `if (!m_ImpactEffect || !m_ImpactEffectAnimator) return` after already checking `m_ImpactEffect != null`. Simplify.
  - `m_ImpactEffectAnimator.Play(0, 0, 0f)` uses layer and state index; ensure that’s the intended state. Using a state name is often clearer: e.g. `Play("Impact", 0, 0f)`.
- **Material instance:** Assigning `m_EnemySprite.material = m_FlashMaterial` can create an instance and leak if done repeatedly. For a short flash it’s often acceptable; if you see material leaks, consider a shared “flash” material or resetting to a cached shared material.
- **Audio:** `PlayHitSound()` already null-checks `m_EnemySoundSource`; also guard `m_HitSound` to avoid errors.

---

## 8. Animation / assets (PR 417)

- PR 417 adds 4-direction attack anims (PlayerAttackDown/Left/Right/Up), new Dante attack sheet, and Enemy Impact + FlashMaterial. Ensure all new assets are assigned in prefabs and that the Player controller has the correct state names (`AttackUp`, `AttackDown`, `AttackLeft`, `AttackRight`) to match `PlayerCombat`.
- `.idea/` files are in the diff – consider adding `.idea/` to `.gitignore` so IDE settings aren’t committed.

---

## Checklist before or right after merge

- [ ] **PlayerCombat:** Restore or reimplement attack point movement with player direction (or document that it’s handled elsewhere).
- [ ] **EnemyHealth:** Null-check `m_HitReaction` before calling FlashWhite/PlayHitSound/ImpactEffect; fix “hit pints” typo; remove unused `Unity.Mathematics` if applicable.
- [ ] **PlayerHealth:** Restore or replace game-over/death flow (e.g. `StartPlayerDeath()` / `ResetHealth()` or equivalent); clamp health in `Hit()`; fix typos.
- [ ] **PlayerAnimator:** Confirm no other code relies on animator `LastDirX`/`LastDirY`; remove duplicate animator init (Start vs Awake).
- [ ] **CameraShake:** Remove Debug.Log and empty Update; consider 2D shake and multi-frame shake.
- [ ] **EnemyHitScript:** Remove unused using; add null checks for sprite, material, impact effect, and audio; simplify ImpactEffect logic; consider state name for `Play()`.
- [ ] **PR / repo:** Add `.idea/` to `.gitignore` if the team doesn’t commit IDE config.

---

## What’s working well

- Clear 4-direction attack and use of `LastMovedDirection`.
- Hit feedback (flash, sound, impact) is separated into `EnemyHitScript` and is easy to extend.
- PlayerCombat Tooltips and optional input flag.
- PlayerHealth Heal/Hit split and extra events for UI/feedback.
- Use of UnityEvents for hit and health events.

Thanks for the combat pass—addressing the points above will make the merge safe and consistent with main’s game-over and attack behavior.
