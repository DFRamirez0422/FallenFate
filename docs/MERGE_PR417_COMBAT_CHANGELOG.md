# Merge PR #417 (Combat feedback first pass) – What changed

This document describes **only the script changes** merged from Erik and Mike’s Combat feedback (PR #417). Hitbox position is driven by the animation editor, so attack-point movement in code was not restored.

---

## New scripts (added)

### 1. `Assets/Sandbox/ErikA/Scripts/CameraShake.cs`

- **Purpose:** Optional camera shake on demand (e.g. on hit or impact).
- **Usage:** Add to main Camera; call `PlayShake()` from events (e.g. `EnemyHealth.m_OnHit` or combat events).
- **Details:**
  - Stores initial position in `Awake()`.
  - `PlayShake()` applies a 2D offset (`Random.insideUnitCircle * m_ShakeAmount`) and keeps camera Z so it works in 2D.
  - No `Update()`, no `Debug.Log` (cleaned up from PR version).

### 2. `Assets/Sandbox/ErikA/Scripts/DisableHitEffect.cs`

- **Purpose:** Disable a hit/impact effect GameObject (e.g. after an animation or timer).
- **Usage:** On your impact effect prefab; call `DisableEffect()` from an animation event or script to set the object inactive.
- **Details:** Single method: `gameObject.SetActive(false)`.

### 3. `Assets/Sandbox/ErikA/Scripts/EnemyHitScript.cs`

- **Purpose:** Central place for enemy hit feedback: white flash, hit sound, and impact VFX.
- **Usage:** Add to enemies that use `EnemyHealth`. Assign in Inspector: hit sound, optional AudioSource, impact effect prefab, flash material, sprite renderer (or leave empty to use same object’s).
- **Details:**
  - **FlashWhite():** Swaps sprite material to `m_FlashMaterial` for `m_HitDuration`, then restores original.
  - **PlayHitSound():** `PlayOneShot(m_HitSound)` on `m_EnemySoundSource` (with null checks).
  - **ImpactEffect():** Enables `m_ImpactEffect` and plays animator state at index 0.
  - All methods null-check so enemies without this component or without optional fields don’t throw. Unused `Unity.VisualScripting` removed.

---

## Pre-existing scripts – detailed changes

### 4. `Assets/Scripts/Player/PlayerCombat.cs`

| What | Before (main) | After (merge) |
|------|----------------|----------------|
| **Attack direction** | Single “IsAttacking” bool; animator decided animation. | **4-direction attack:** Chooses `AttackUp` / `AttackDown` / `AttackLeft` / `AttackRight` from `PlayerAnimator.LastMovedDirection` and calls `m_Animator.Play(state)`. |
| **Attack point** | `MoveAttackPoint()` in `Update()` using `PlayerMovement.CurrentDirection` to move `m_AttackPoint`. | **Removed.** Hitbox position is driven by the animation editor; no code movement of attack point. |
| **Dependencies** | `PlayerMovement`, `Animator`. | **`PlayerAnimator`** (for `LastMovedDirection`), **Animator**. Optional **AudioSource** and **AudioClip** for swing sound. |
| **Input / cooldown** | Same: `m_HandleInput` and “Attack” button, cooldown timer. | Same. |
| **DealDamage / FinishAttacking** | Unchanged. | Unchanged. |
| **New fields** | — | `m_PlayerAnimator`, `m_AttackUpState`, `m_AttackDownState`, `m_AttackLeftState`, `m_AttackRightState`, `m_PlayerAudio`, `m_AttackSwingClip`. |

**Summary:** Combat now plays direction-based attack states and optional attack swing audio. Attack point is no longer moved in code.

---

### 5. `Assets/Scripts/Enemy/EnemyHealth.cs`

| What | Before (main) | After (merge) |
|------|----------------|----------------|
| **Init** | `Start()` set `m_CurrentHealth = m_MaxHealth`. | **`Awake()`** sets current health and caches **`EnemyHitScript`** (`GetComponent<EnemyHitScript>()`). |
| **ChangeHealth** | Added amount, clamped to max, destroyed if ≤ 0. | Same logic, plus: **`m_OnHit?.Invoke()`**, then **`m_HitReaction?.FlashWhite()`**, **`PlayHitSound()`**, **`ImpactEffect()`** (all null-safe so no error if component missing). |
| **New** | — | **`UnityEvent m_OnHit`** (Inspector), optional **EnemyHitScript** for flash/sound/impact. |
| **Removed** | — | `using Unity.Mathematics` (unused). |
| **Typo** | “hit pints” | “hit points”. |

**Summary:** Enemies can now trigger hit events and, if they have `EnemyHitScript`, show flash, play sound, and spawn impact effect. Safe if `EnemyHitScript` is not present.

---

### 6. `Assets/Scripts/Player/PlayerAnimator.cs`

| What | Before (main) | After (merge) |
|------|----------------|----------------|
| **Direction storage** | None. | **`m_LastMovedDirection`** (default `Vector2.down`) and **`LastMovedDirection`** property. |
| **SetCurrentDirection** | Only set animator floats: `LastDirX`, `LastDirY` from `direction.normalized`. | **Also** updates `m_LastMovedDirection` using **`QuantizeTo8(direction)`** (nearest 45°). Still sets **`LastDirX`** and **`LastDirY`** so existing blend trees keep working. |
| **New helper** | — | **`QuantizeTo8(Vector2)`** – normalizes and snaps angle to 45° steps for 8-direction facing. |
| **Init** | Both `Start()` and `Awake()` assigned `m_Animator`. | **Only `Awake()`** assigns `m_Animator`. |
| **StartAnimation / Reset** | Unchanged. | Unchanged. |

**Summary:** Last move direction is stored and quantized for combat (and anything else that needs facing). Animator parameters for movement are unchanged.

---

### 7. `Assets/Scripts/Player/PlayerHealth.cs`

| What | Before (main) | After (merge) |
|------|----------------|----------------|
| **Events** | `m_OnZeroHealth`, `m_GameOverScreenPrefab`. | **Added:** **`m_OnHeal`**, **`m_OnHit`**. Kept: **`m_OnZeroHealth`**, **`m_GameOverScreenPrefab`**. |
| **ChangeHealth** | `m_CurrentHealth += amount`; if ≤ 0 invoked `m_OnZeroHealth`. | **Split:** positive amount → **`Heal(amount)`**, negative → **`Hit(-amount)`**. After that, if **`m_CurrentHealth <= 0`** then **`m_OnZeroHealth?.Invoke()`** (game over flow unchanged). |
| **New private methods** | — | **`Heal(int)`:** adds, clamps to max, invokes **`m_OnHeal`**. **`Hit(int)`:** subtracts, clamps to 0, invokes **`m_OnHit`**. |
| **Zero health** | Only `m_OnZeroHealth`. | Same: still only **`m_OnZeroHealth`** (no `SetActive(false)`). **`StartPlayerDeath()`** and **`ResetHealth()`** kept as on main. |
| **StartPlayerDeath** | Disable movement, play “Death”, instantiate GameOver prefab. | **Unchanged.** |
| **ResetHealth** | Set current health back to max. | **Unchanged.** |
| **Properties** | — | **`CurrentHealth`**, **`MaxHealth`** (read-only) for UI or other systems. |

**Summary:** Health now separates heal vs damage and exposes OnHeal/OnHit for UI/feedback. Game over, death sequence, and reset behavior are unchanged from main.

---

## What was not merged

- **Animation assets / controller:** No Dante or Enemy animation files or controller changes were merged (only scripts). You can bring in PR #417’s 4-direction attack anims and controller when ready.
- **ErikA assets:** No prefabs, materials, or audio from PR #417 (e.g. ImpactEffect prefab, FlashMaterial, SFX) were copied. Add those from the PR branch or your art repo as needed.
- **.idea/:** No IDE config from the PR was merged.
- **JoseE scene / other scenes:** No scene file changes from PR #417.

---

## Setup after merge

1. **Player:** Ensure the Animator has states **AttackUp**, **AttackDown**, **AttackLeft**, **AttackRight** (or match the names in `PlayerCombat`). Assign **PlayerAnimator** and, optionally, **AudioSource** + **Attack Swing Clip** on the player.
2. **Enemies:** Add **EnemyHitScript** to enemies that should flash/play sound/impact. Assign hit sound, impact effect prefab, and flash material. Leave optional fields unset if not used.
3. **Camera (optional):** Add **CameraShake** to the main camera and call **PlayShake()** from `m_OnHit` or another event if you want screen shake on hit.
4. **Impact effects:** Use **DisableHitEffect** on the impact effect prefab and call **DisableEffect()** from an animation event when the effect is done.

If you want, next step can be wiring one enemy and one impact prefab from PR #417 into a test scene and double-checking flash/sound/VFX.