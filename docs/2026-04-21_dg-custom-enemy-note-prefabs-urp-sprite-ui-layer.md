# David Glazier
## DG custom / enemy note prefabs — UI layer + URP Sprite-Unlit-Default
Date: April 21, 2026

## Change descriptions
All **18** note prefabs under `CustomNotes` and `Enemy_notes` were aligned for **render-texture / UI-style capture**: every **GameObject** that was on the **Default** layer is now on the **UI** layer (index **5**). Every **`SpriteRenderer`** material slot that used built-in **Sprites-Default** or various **URP** sprite materials was pointed at the package **`Sprite-Unlit-Default.mat`** from the project’s installed URP package.

## Technical explanation
- **Layer 5 (UI)** is the built-in Unity **UI** layer so a camera that renders only **UI** (typical for a **RenderTexture** used as a UI texture) will still see these world-space / hybrid note visuals.
- **Material reference** uses the asset GUID from  
  `Library/PackageCache/com.unity.render-pipelines.universal@2b88762731f8/Runtime/Materials/Sprite-Unlit-Default.mat.meta`  
  → **`guid: 9dfc825aed78fcd4ba02077103263b40`**.  
  If URP is upgraded and the package hash or GUID changes, reassign the material in the Editor or refresh this GUID from the new `.meta` file.
- **TextMeshPro** and other **UI** elements on counter notes were already on layer 5; **TMP** default material was left as `m_Material: {fileID: 0}` (TMP asset handles drawing).

## File location tracking
| Area | Path |
|------|------|
| Player custom note prefabs (9) | `Assets/Sandbox/David_G/DG_RythmTests/DG_GuiatatBattle_scripts/CustomNotes/*.prefab` |
| Enemy note prefabs (9) | `Assets/Sandbox/David_G/DG_RythmTests/DG_GuiatatBattle_scripts/Enemy_notes/*.prefab` |
| URP Sprite-Unlit-Default (source of GUID) | `Library/PackageCache/com.unity.render-pipelines.universal@2b88762731f8/Runtime/Materials/Sprite-Unlit-Default.mat` |

## Communication context
**David** owns these sandbox prefabs; anyone wiring **RT cameras** or **layer masks** for the guitar lane should confirm the main gameplay camera still includes **layer 5** if notes must also appear outside the RT.
