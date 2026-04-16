# David Glazier
## StatTracker health sync across Act 3 scene loads
Date: April 14, 2026

## Change Descriptions

`StatTracker` was not reliably re-binding to the new `PlayerHealth` when entering Act 3 scenes (`1 Solitary Confinment`, `2 Cell Room`, `3 Nightmare`). The carried `lastPlayerHealth` value could stop updating because `SceneManager.sceneLoaded` ran before the Player instance was consistently discoverable with `FindObjectOfType`, leaving `PlayerObject` null and skipping `PlayerStats()`. A one-frame deferred resolve using the `Player` tag (with fallback) fixes binding; null-safe apply removes a possible NRE when respawning after death.

**Act 3 “always full health (4)”:** `Main Menu/MainMenu.unity` already contains a `StatTracker` (`DontDestroyOnLoad`). `Act 3/1 Solitary Confinment.unity` also places a second object `HealthTracker` with `StatTracker`. That new instance starts with `lastPlayerHealth == 0`, so `IsAlive` is false and `ApplyCarriedHealthToPlayer` assigns `m_MaxHealth` (4)—often **after** the first tracker applied the real value, overwriting it. A **singleton** in `Awake` destroys duplicate instances before they subscribe to `sceneLoaded`.

## Technical Explanations

- `OnSceneLoaded` starts a coroutine that `yield return null` once so the Player prefab is present and components are initialized before lookup.
- `ResolvePlayerHealth()` prefers `GameObject.FindGameObjectWithTag("Player")` + `GetComponent<PlayerHealth>()`, then falls back to `FindObjectOfType<PlayerHealth>()`.
- `ApplyCarriedHealthToPlayer()` returns early if no player was found; alive players get `lastPlayerHealth`, dead carry uses max health on the new instance.
- **Singleton:** only one `StatTracker` may exist; extras are `Destroy`ed in `Awake` so they never register `sceneLoaded` or reset HP.
- Scene comparison: **`Act 1 REMAKE.unity` has no `StatTracker` in the scene file** (carry comes from Menu’s DDOL instance). **`Act 2 Main Scene.unity` also has none**; `Act 2/Garden.unity` does include one. Player prefab overrides: Act 1 REMAKE sets `m_MaxHealth` to **4** and wires extra `m_OnHit` / `m_OnHeal` UnityEvents (e.g. `HealthBar`); Solitary only overrides `m_MaxHealth` to **4** with no extra health UI wiring in the snippet—nothing in either scene sets serialized `m_CurrentHealth` on the instance (prefab default **0** until scripts run).

## File Location Tracking

No files were moved between branches.

## Communication Context

Erik’s autosave / player stat tooling; level designers using Act 3 should see consistent HP when moving between those scenes after play once `StatTracker` exists in the session (e.g. from `1 Solitary Confinment` or an earlier scene that contains it).

## Prevention (regression)

- **Singleton** destroys extra `StatTracker` instances and logs a **Console warning** naming the scene/object.
- **Editor:** **Tools → Fallen Fate → Validate StatTracker In Build Scenes** — errors if one `.unity` file references `StatTracker` more than once; warns if multiple **enabled** build scenes each contain a `StatTracker` (policy: prefer one bootstrap, e.g. Main Menu).
- **Build:** `StatTrackerBuildAudit` runs on preprocess and **fails the build** on duplicate StatTrackers in a single scene file.
- **Scene:** Removed duplicate `HealthTracker` from `Act 3/1 Solitary Confinment.unity` and from `Act 2/Garden.unity`; `-Main Scenes` now only references `StatTracker` on `Main Menu/MainMenu.unity`.
- **Cursor:** `.cursor/rules/stat-tracker-singleton.mdc` reminds agents not to add second `StatTracker` objects to level scenes.

# File Locations

| File / Asset | Path |
|--------------|------|
| StatTracker | `Assets/Sandbox/ErikA/Scripts/AutosaveScripts/StatTracker.cs` |
| StatTracker build audit (Editor) | `Assets/Sandbox/ErikA/Scripts/Editor/StatTrackerBuildAudit.cs` |
| Cursor rule | `.cursor/rules/stat-tracker-singleton.mdc` |
| Act 3 Solitary (duplicate removed) | `Assets/-Main Scenes/Act 3/1 Solitary Confinment.unity` |
