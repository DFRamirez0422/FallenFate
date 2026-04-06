# David Glazier
## Audio Mixer Cleanup — Remove Duplicate & UI Group
Date: April 05, 2026

---

## Which Mixer to Keep

| Mixer | Path | Status |
|-------|------|--------|
| **MainAudioMixer** ✅ KEEP | `Assets/audio/SoundPrefabs/MainAudioMixer.mixer` | **Active — all production assets reference this** |
| **Main Mixer** ❌ DELETE | `Assets/Scripts/Player/Main Mixer.mixer` | Unused in production (only in Michael's sandbox demo) |

**Why:** Every `SoundDefinition` asset in the project (SFX and Music) points to `MainAudioMixer` by GUID. `Main Mixer` is only referenced inside Michael's sandbox demo scene/prefab and is in the wrong folder (`Scripts/Player/`).

---

## UI Group Removal — What Breaks

The `MainAudioMixer` currently has 4 groups: **Master, SoundFX, Music, UI**.
Removing the UI group and routing UI sounds to SoundFX instead.

### Nothing in Production Breaks

| Area | Finding |
|------|---------|
| Scripts (`SoundMixerManager.cs`) | No `SetUIVolume` method exists — UI volume is already unused in code |
| SoundDefinition assets | Zero assets point to the UI mixer group |
| Scenes & Prefabs | No `AudioSource` in any scene or prefab is routed to the UI group |
| Exposed parameter `UIVolume` | Exists in the mixer but no script calls it |

**Bottom line: Removing the UI group breaks nothing in the current project.**

---

## What Needs Manual Updating

### ⚠️ Michael H — Sandbox Demo Files
These files reference the **old `Main Mixer`** that is being deleted. They are sandbox/demo only, not in production builds.

| File | Issue |
|------|-------|
| `Assets/Sandbox/MichaelH/Prefabs/SoundFXObject.prefab` | References `Main Mixer` GUID — will show missing mixer after deletion |
| `Assets/Sandbox/MichaelH/Scenes/DemoForSoundManager.unity` | References `Main Mixer` GUID — will show missing mixer after deletion |

**Action needed:** Michael should re-assign the `AudioMixer` reference in his demo scene and prefab to point to `MainAudioMixer`.

---

## Steps to Complete the Cleanup (in Unity Editor)

1. **Delete** `Assets/Scripts/Player/Main Mixer.mixer` (and its `.meta`)
2. **Open** `MainAudioMixer` in the Audio Mixer window
3. **Delete** the `UI` mixer group
4. **Remove** the exposed parameter `UIVolume` from the mixer's exposed parameters list
5. Any future UI sounds → assign `SoundFX` mixer group in their `SoundDefinition` asset

---

## File Locations

| File / Asset | Path |
|--------------|------|
| MainAudioMixer (KEEP) | `Assets/audio/SoundPrefabs/MainAudioMixer.mixer` |
| Main Mixer (DELETE) | `Assets/Scripts/Player/Main Mixer.mixer` |
| SoundMixerManager | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundMixerManager.cs` |
| SoundFXManager | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundFXManager.cs` |
| MusicManager | `Assets/Sandbox/MichaelH/Scripts/Sound/MusicManager.cs` |
| SoundDefinition | `Assets/Sandbox/MichaelH/Scripts/Sound/SoundDefinition.cs` |
| SoundFXObject.prefab (needs update) | `Assets/Sandbox/MichaelH/Prefabs/SoundFXObject.prefab` |
| DemoForSoundManager.unity (needs update) | `Assets/Sandbox/MichaelH/Scenes/DemoForSoundManager.unity` |
