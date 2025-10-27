---
author: Jose E.
description: Brief Description of the Rhythm System
date: 2025-10-22
---

The Rhythm System is one of the main gameplay pillars. It ensures combat feels musical, rewarding, and skill-based. Players attack on beat, chaining perfect hits into powerful finishers and cinematic Quick-Time Events (QTEs)

# Prefabs and Scripts

- `MusicManager`
    - Game object that managers the background music as well as the rhythm subsystem.
    - `MusicAutoStarter`
        - Used for development. Automatically starts the given song as soon as the scene is initialized.
    - `RhythmMusicPlayer`
        - Handles the music playback via a speaker object, loop controls, and a tempo speed.
    - `MusicDictionary`
        - Stores a list of all music files to be mapped to a designer-friendly name and associated with a tempo.
- `MusicStaff`
    - Game object for the music staff at the bottom of the screen.
    - `MusicBarUI`
        - Controls the appearance of the beat markers and central graphic on the music staff.
- `RhythmBonusJudge`
    - Game object that decides whether something was on beat or off beat based on the given `MusicManager` object.
    - `RhythmBonusJudge`
        - Controls for the timing, calibration, and bonus multipliers.
- `BeatComboCounter`
    - Script that bridges between the rhythm system and the rest of the codebase by performing queries and, if needed, automatic combo counting.

# How-To

Please read over [[How to Add Songs|this document]] to add music for the rhythm system.