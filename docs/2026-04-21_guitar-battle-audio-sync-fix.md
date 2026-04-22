# David Glazier
## Guitar Battle Audio Sync Fix
Date: April 21, 2026

---

## Table of Contents
- [Problem](#problem)
- [Root Cause Analysis](#root-cause-analysis)
- [Changes Made](#changes-made)
- [Technical Explanation](#technical-explanation)
- [File Locations](#file-locations)

---

## Problem

Playtesters reported that notes were out of sync with the music in two guitar battle scenes:

| Scene | Reported Delay |
|-------|---------------|
| `GuitarBattle_after_shed` (Act 1) | ~2 seconds |
| `GuitarBattle_Rooftop` (Act 3) | ~1 second |

The `GuitarBattle_Exit Door` (Act 2) scene was confirmed to be properly in time.

## Root Cause Analysis

Two factors combined to cause the desync:

### 1. `preloadAudioData` was disabled on all guitar battle audio files

All five `.wav` files used by guitar battles had `preloadAudioData: false` in their Unity import settings. Per the [Rhythm Timeline 2 Documentation (p.64)](../Assets/Dypsloom/RhythmTimeline/Rhythm%20Timeline%202%20Documentation.pdf), audio clips must use **"Decompress On Load" and "Preload Audio Data"** for proper DSP clock synchronization.

With `preloadAudioData: false`, Unity defers loading and decompressing audio until the first time it's needed. When the PlayableDirector starts the timeline, the DSP clock begins ticking and notes start moving, but the audio system has to load/decompress the file before it can play — creating a gap between visual notes and audible music.

### 2. Audio clip start position in the timeline

| Scene | Audio Clip Start Time in Timeline | Result |
|-------|-----------------------------------|--------|
| **Exit Door (GOOD)** | **4.93 seconds** | 4.93s buffer gives Unity time to load audio even without preload |
| **After Shed (BAD)** | **0 seconds** | No buffer — audio must play immediately but isn't loaded yet |
| **Rooftop (BAD)** | **0 seconds** | No buffer — same issue as After Shed |

The Exit Door scene accidentally avoided the bug because its audio track starts 4.93 seconds into the timeline, giving Unity enough time to load the audio data before it's actually needed. The other two scenes have audio starting at time 0, requiring immediate playback that can't happen with unloaded data.

## Changes Made

Enabled `preloadAudioData` on all five guitar battle audio files. This ensures audio data is fully loaded and decompressed during scene load, before the PlayableDirector starts the timeline.

**Before:** `preloadAudioData: 0` (false)
**After:** `preloadAudioData: 1` (true)

This was applied to all guitar battle audio files for consistency, including the Exit Door audio (which worked before but was technically misconfigured — it would break if its timeline audio was ever moved to time 0).

## Technical Explanation

The Rhythm Timeline system uses Unity's **DSP Clock** update mode on the PlayableDirector. Both note positions and audio playback are driven by the same DSP clock, ensuring frame-accurate sync. However, this only works when audio data is immediately available. The PlayableDirector advances time based on DSP regardless of whether the AudioSource has data ready to play.

With `preloadAudioData: true`, Unity loads and decompresses the audio clip during scene loading (before `Awake`/`Start`), so the audio buffer is populated before the timeline begins. This eliminates the loading delay that was desynchronizing notes from music.

### Scene Configuration Reference (from the properly-timed Exit Door scene)

| Property | Exit Door (Good) | After Shed (Was Bad) | Rooftop (Was Bad) |
|----------|-----------------|---------------------|-------------------|
| BPM | 145 | 105 | 75 |
| NoteSpeed | 0.17 | 0.1 | 0.04 |
| ScaleNoteSpeedToBpm | true | true | true (prefab default) |
| Effective NoteSpeed | 24.65 | 10.5 | 3.0 |
| Audio Start in Timeline | 4.93s | 0s | 0s |
| preloadAudioData (fixed) | true | true | true |

## File Locations

| File / Asset | Path |
|--------------|------|
| Level1Track2_Comfort.wav.meta | `Assets/audio/GuitarBattleSongs/Level1Track2_Comfort.wav.meta` |
| Level1Track1_Comfort.wav.meta | `Assets/audio/GuitarBattleSongs/Level1Track1_Comfort.wav.meta` |
| Level2Track3_RefuseDread.wav.meta | `Assets/audio/GuitarBattleSongs/Level2Track3_RefuseDread.wav.meta` |
| Level3Track4_FallenFate(Distort).wav.meta | `Assets/audio/GuitarBattleSongs/Level3Track4_FallenFate(Distort).wav.meta` |
| Level3Track4_FallenFate.wav.meta | `Assets/audio/GuitarBattleSongs/Level3Track4_FallenFate.wav.meta` |
| GuitarBattle_after_shed (scene) | `Assets/-Main Scenes/Act 1/Act1AfterGuitar/GuitarBattle_after_shed.unity` |
| GuitarBattle_Exit Door (scene) | `Assets/-Main Scenes/Act 2/GuitarBattle_Exit Door.unity` |
| GuitarBattle_Rooftop (scene) | `Assets/-Main Scenes/Act 3/GuitarBattle_Rooftop.unity` |
| Dante_After_Level1 (timeline) | `Assets/Audio/GuitarBattleSongs/Dante_After_Level1.asset` |
| Dante_Level2 (timeline) | `Assets/Audio/GuitarBattleSongs/Dante_Level2.asset` |
| LastMusicDante (timeline) | `Assets/audio/GuitarBattleSongs/LastMusicDante.asset` |
| RhythmDirector.cs | `Assets/Dypsloom/RhythmTimeline/Scripts/Core/Managers/RhythmDirector.cs` |
| Rhythm Director prefab | `Assets/Dypsloom/RhythmTimeline/Demos/Shared/Prefabs/Rhythm Director.prefab` |
| Rhythm Timeline 2 Docs | `Assets/Dypsloom/RhythmTimeline/Rhythm Timeline 2 Documentation.pdf` |
