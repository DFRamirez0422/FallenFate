# David Glazier
## GuitarBattle Tutorial Duel — double / out-of-sync song audio
Date: April 21, 2026

## Change descriptions
The tutorial duel scene sounded like two copies of the song playing slightly out of sync. The Dypsloom **Rhythm Director** prefab’s **AudioSource** has **Play On Awake** enabled while the scene also assigns the **full song clip** to that source. Unity therefore starts playback at **Awake**, and **`RhythmDirector`** starts the same audio again via the **Timeline** in **`Start`**, which produces two overlapping streams with a small timing offset.

A scene override was added so the primary Rhythm Director instance’s **AudioSource** has **Play On Awake** turned off; only the Timeline-driven path should play the music.

## Technical explanation
- **`RhythmDirector`** (`PlaySong` → `PlayableDirector.Play()`) binds timeline **AudioTrack** output to **`m_AudioSources`** and drives playback on the DSP clock.
- The shared **Rhythm Director** prefab sets **`m_PlayOnAwake: 1`** on its **AudioSource** while **`m_audioClip`** is left empty in the prefab. In **GuitarBattle_Tutorial Duel**, the scene assigns **`m_Resource` / `m_audioClip`** to the song, so the source is no longer “silent until the timeline runs.”
- Result: **Awake** autoplay + **Start** timeline play on the same clip ≈ doubled, desynced audio.

The scene also contains two **Rhythm Director** prefab instances (**Rhythm Director** and **Rhythm Director (1)**) for two players, both with **`m_PlayOnStart: 1`**. The second instance disables its **AudioSource** in this scene, so it was not the main cause of the duplicate here. If both sources were active with the same clip, two directors would also double the mix.

## File location tracking
| File / Asset | Path |
|--------------|------|
| GuitarBattle_Tutorial Duel scene | `Assets/-Main Scenes/Act 1/GuitarBattle_Tutorial Duel.unity` |
| Rhythm Director prefab (default Play On Awake on AudioSource) | `Assets/Dypsloom/RhythmTimeline/Demos/Shared/Prefabs/Rhythm Director.prefab` |
| RhythmDirector script | `Assets/Dypsloom/RhythmTimeline/Scripts/Core/Managers/RhythmDirector.cs` |

## Communication context
Anyone maintaining **Act 1 guitar battle** scenes or **Dypsloom Rhythm Director** prefab overrides should know: **do not** leave **Play On Awake** on for sources that are both clip-filled and driven by the rhythm timeline, unless you intend a deliberate layered effect.
