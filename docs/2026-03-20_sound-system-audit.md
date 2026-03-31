# Sound System Compliance Audit
## Non-Compliant Script Report
Date: March 20, 2026

---

## Overview

A full audit of every `.cs` script in the project was performed against the [Sound System documentation](../c:/Users/dglaz/Downloads/SoundSystem%20(5).md).

**The only correct way to play SFX is:**
```csharp
[SerializeField] private SoundDefinition m_AttackSwingSfx;

if (m_AttackSwingSfx != null)
    SoundFXManager.instance.Play(m_AttackSwingSfx, transform);
```

### Key Rules Checked

| Rule | Source |
|------|--------|
| Do NOT use raw `AudioClip` fields in gameplay scripts | Doc §3 |
| Do NOT use `AudioSource.PlayOneShot()` or `AudioSource.Play()` directly | Doc §8 |
| Do NOT use the deprecated `PlaySoundFXClip()` method | Doc §8 |
| Use `SoundDefinition` ScriptableObjects for all sound data | Doc §2 |
| Route playback exclusively through `SoundFXManager.instance.Play()` | Doc §8 |

---

## Non-Compliant Scripts — Summary Table

| Script | Path | Developer | Non-Compliant Lines | Issue |
|--------|------|-----------|---------------------|-------|
| `Powered_Door.cs` | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/Powered_Door.cs` | Angel Rodriguez | 18, 23, 44 | Raw `AudioSource` field + `GetComponent<AudioSource>()` + direct `.Play()` call |
| `OpenDoors.cs` | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/OpenDoors.cs` | Angel Rodriguez | 9, 24, 66 | Raw `AudioSource` field + `GetComponent<AudioSource>()` + direct `.Play()` call |
| `PlayerSound.cs` | `Assets/Scripts/Player/PlayerSound.cs` | JoseEscobedo02 | 14, 18–32, 62, 68, 74, 80, 86 | Raw `AudioSource` + 6 raw `AudioClip` fields + multiple `PlayOneShot()` calls throughout |
| `NPCTalk.cs` | `Assets/Scripts/NPC/NPCTalk.cs` | Epicdavid12 | 18, 20, 34–37, 66 | Raw `AudioSource` + raw `AudioClip` + `PlayOneShot()` |
| `DialogueTrigger.cs` | `Assets/Scripts/NPC/DialogueTrigger.cs` | JoseEscobedo02 | 30, 37, 43, 54, 92 | Raw `AudioClip` + raw `AudioSource` + two `PlayOneShot()` calls |
| `WardenJumpScare.cs` | `Assets/Sandbox/Nathan White/Nathan's Scripts/Warden Scripts/WardenJumpScare.cs` | NathanFWhite | 9, 10, 51–54 | Raw `AudioClip` + raw `AudioSource` fields; entire sound block commented out — no `SoundFXManager` call exists |
| `EnemyHitScript.cs` | `Assets/Sandbox/ErikA/Scripts/EnemyHitScript.cs` | Boobombdigity (Erik A) | 6, 30, 52–53 | Leftover `AudioSource` field + auto-assign in `Awake`; old `PlayOneShot` commented out but vestige fields remain *(playback is correctly using SoundFXManager — partial compliance)* |

---

## Detailed Breakdown Per Script

---

### 1. `Powered_Door.cs`
**Developer:** Angel Rodriguez
**Path:** `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/Powered_Door.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 18 | `private AudioSource _doorOpenSound;` | Raw `AudioSource` field. Should be `SoundDefinition`. |
| 23 | `_doorOpenSound = GetComponent<AudioSource>();` | Retrieving `AudioSource` component directly. |
| 44 | `_doorOpenSound.Play();` | Calling `.Play()` directly on an `AudioSource`. Must use `SoundFXManager.instance.Play(soundDef, transform)`. |

**Fix Required:**
```csharp
// Replace field:
[SerializeField] private SoundDefinition m_DoorOpenSfx;

// Remove GetComponent<AudioSource>() in Start()

// Replace playback:
if (m_DoorOpenSfx != null)
    SoundFXManager.instance.Play(m_DoorOpenSfx, transform);
```

---

### 2. `OpenDoors.cs`
**Developer:** Angel Rodriguez
**Path:** `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/OpenDoors.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 9 | `private AudioSource _doorOpenSound;` | Raw `AudioSource` field. Should be `SoundDefinition`. |
| 24 | `_doorOpenSound = GetComponent<AudioSource>();` | Retrieving `AudioSource` component directly. |
| 66 | `_doorOpenSound.Play();` | Calling `.Play()` directly on an `AudioSource`. |

**Fix Required:**
```csharp
// Replace field:
[SerializeField] private SoundDefinition m_DoorOpenSfx;

// Remove GetComponent<AudioSource>() in Awake()

// Replace playback in OpenDoor():
if (m_DoorOpenSfx != null)
    SoundFXManager.instance.Play(m_DoorOpenSfx, transform);
```

---

### 3. `PlayerSound.cs`
**Developer:** JoseEscobedo02
**Path:** `Assets/Scripts/Player/PlayerSound.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 14 | `[SerializeField] private AudioSource m_SoundPlayer;` | Raw `AudioSource`. Should not be used. |
| 18 | `[SerializeField] private AudioClip m_WalkNormalSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 20 | `[SerializeField] private AudioClip m_WalkSnowSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 22 | `[SerializeField] private AudioClip m_WalkStairSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 24 | `[SerializeField] private AudioClip m_WalkBushSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 26 | `[SerializeField] private AudioClip m_AttackSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 28 | `[SerializeField] private AudioClip m_CollisionSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 30 | `[SerializeField] private AudioClip m_WallHitSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 32 | `[SerializeField] private AudioClip m_DamageSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 62 | `m_SoundPlayer.PlayOneShot(sound_clip);` | `PlayOneShot()` is deprecated. Use `SoundFXManager`. |
| 68 | `m_SoundPlayer.PlayOneShot(m_CollisionSound);` | `PlayOneShot()` is deprecated. |
| 74 | `m_SoundPlayer.PlayOneShot(m_WallHitSound);` | `PlayOneShot()` is deprecated. |
| 80 | `m_SoundPlayer.PlayOneShot(m_DamageSound);` | `PlayOneShot()` is deprecated. |
| 86 | `m_SoundPlayer.PlayOneShot(m_AttackSound);` | `PlayOneShot()` is deprecated. |

**Note to team:** This entire script is a legacy sound wrapper. All calls to `PlayerSound` methods from other scripts (e.g., `PlayerCombat` already has this commented out) should be migrated. Each `AudioClip` field should become a `SoundDefinition` field, and each `PlayOneShot` call should become `SoundFXManager.instance.Play(soundDef, transform)`.

---

### 4. `NPCTalk.cs`
**Developer:** Epicdavid12
**Path:** `Assets/Scripts/NPC/NPCTalk.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 18 | `[SerializeField] private AudioSource m_SoundPlayer;` | Raw `AudioSource` field. |
| 20 | `[SerializeField] private AudioClip m_TalkSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 34–37 | `if (!m_SoundPlayer) { m_SoundPlayer = GetComponent<AudioSource>(); }` | Fallback `GetComponent` retrieval. |
| 66 | `m_SoundPlayer.PlayOneShot(m_TalkSound);` | `PlayOneShot()` is deprecated. |

**Fix Required:**
```csharp
// Replace fields:
[SerializeField] private SoundDefinition m_TalkSfx;

// Remove AudioSource field and GetComponent fallback in Awake()

// Replace playback:
if (m_TalkSfx != null)
    SoundFXManager.instance.Play(m_TalkSfx, transform);
```

---

### 5. `DialogueTrigger.cs`
**Developer:** JoseEscobedo02
**Path:** `Assets/Scripts/NPC/DialogueTrigger.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 30 | `[SerializeField] private AudioClip m_TalkSound;` | Raw `AudioClip`. Must be `SoundDefinition`. |
| 37 | `private AudioSource m_SoundPlayer;` | Raw `AudioSource` field. |
| 43 | `m_SoundPlayer = GetComponent<AudioSource>();` | Direct component retrieval. |
| 54 | `m_SoundPlayer.PlayOneShot(m_TalkSound);` | `PlayOneShot()` in `Update()` — deprecated. |
| 92 | `m_SoundPlayer.PlayOneShot(m_TalkSound);` | `PlayOneShot()` in `OnTriggerEnter2D()` — deprecated. |

**Fix Required:**
```csharp
// Replace fields:
[SerializeField] private SoundDefinition m_TalkSfx;

// Remove AudioSource field, remove GetComponent in Awake(), remove [RequireComponent(typeof(AudioSource))]

// Replace both playback calls:
if (m_TalkSfx != null)
    SoundFXManager.instance.Play(m_TalkSfx, transform);
```

---

### 6. `WardenJumpScare.cs`
**Developer:** NathanFWhite
**Path:** `Assets/Sandbox/Nathan White/Nathan's Scripts/Warden Scripts/WardenJumpScare.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 9 | `public AudioClip jumpscareClip1;` | Raw `AudioClip` field. Must be `SoundDefinition`. |
| 10 | `public AudioSource jumpscareSource;` | Raw `AudioSource` field. Should be removed. |
| 51–54 | `// jumpscareSource.PlayOneShot(jumpscareClip1);` (commented out) | Old deprecated pattern — and it is still the only sound block. No `SoundFXManager.instance.Play()` exists anywhere in this script. Sound is fully non-functional. |

**Fix Required:**
```csharp
// Replace fields:
[SerializeField] private SoundDefinition m_JumpscareSfx;

// Remove AudioSource field entirely

// Add playback where jumpscare triggers:
if (m_JumpscareSfx != null)
    SoundFXManager.instance.Play(m_JumpscareSfx, transform);
```

---

### 7. `EnemyHitScript.cs` *(Partial — Playback Correct)*
**Developer:** Boobombdigity (Erik A)
**Path:** `Assets/Sandbox/ErikA/Scripts/EnemyHitScript.cs`

| Line(s) | Code | Problem |
|---------|------|---------|
| 6 | `[SerializeField] private AudioSource m_EnemySoundSource;` | Leftover `AudioSource` field. No longer needed — can be removed. |
| 30 | `m_EnemySoundSource = GetComponent<AudioSource>();` | Auto-assigning a component that is no longer used. |
| 52–53 | `// m_EnemySoundSource.PlayOneShot(m_HitSound);` (commented) | Old deprecated pattern left as dead code. |

**Status:** Playback at line 56–57 already correctly uses `SoundFXManager.instance.Play(m_HitSound, transform)`. Only cleanup of leftover fields is needed.

---

## Compliant Scripts (Confirmed Correct)

| Script | Path | Developer |
|--------|------|-----------|
| `PlayerCombat.cs` | `Assets/Scripts/Player/PlayerCombat.cs` | Epicdavid12 |
| `SoundFXManager.cs` | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundFXManager.cs` | Michael Hernandez |
| `SoundDefinition.cs` | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundDefinition.cs` | Michael Hernandez |

---

## File Locations

| File / Asset | Path |
|--------------|------|
| `Powered_Door.cs` | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/Powered_Door.cs` |
| `OpenDoors.cs` | `Assets/Sandbox/Angel_Rodriguez/Resources/Scripts/OpenDoors.cs` |
| `PlayerSound.cs` | `Assets/Scripts/Player/PlayerSound.cs` |
| `NPCTalk.cs` | `Assets/Scripts/NPC/NPCTalk.cs` |
| `DialogueTrigger.cs` | `Assets/Scripts/NPC/DialogueTrigger.cs` |
| `WardenJumpScare.cs` | `Assets/Sandbox/Nathan White/Nathan's Scripts/Warden Scripts/WardenJumpScare.cs` |
| `EnemyHitScript.cs` | `Assets/Sandbox/ErikA/Scripts/EnemyHitScript.cs` |
| `SoundFXManager.cs` | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundFXManager.cs` |
| `SoundDefinition.cs` | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundDefinition.cs` |

---

## Team Members to Notify

| Developer | GitHub Handle | Scripts to Update |
|-----------|--------------|-------------------|
| Angel Rodriguez | `Toxicxeno246` | `Powered_Door.cs`, `OpenDoors.cs` |
| JoseEscobedo02 | `JoseEscobedo02` | `PlayerSound.cs`, `DialogueTrigger.cs` |
| Epicdavid12 | `Epicdavid12` | `NPCTalk.cs` |
| NathanFWhite | `NathanFWhite` | `WardenJumpScare.cs` |
| Boobombdigity (Erik A) | `Boobombdigity` | `EnemyHitScript.cs` (minor cleanup only) |
