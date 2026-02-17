# David Glazier
## Unity Package Resolution Errors -- Prevention Guide
Date: February 17, 2026

---

## Table of Contents
- [What Happened](#what-happened)
- [Why It Happened](#why-it-happened)
  - [Cause 1: Version Mismatches in manifest.json](#cause-1-version-mismatches-in-manifestjson)
  - [Cause 2: EBUSY File Locks from Cursor/VS Code](#cause-2-ebusy-file-locks-from-cursorvs-code)
- [How Builtin Packages Work in Unity 6](#how-builtin-packages-work-in-unity-6)
- [The Fixes That Were Applied](#the-fixes-that-were-applied)
- [Rules to Prevent This From Happening Again](#rules-to-prevent-this-from-happening-again)
- [Recovery Steps If It Happens Again](#recovery-steps-if-it-happens-again)
- [File Locations](#file-locations)

---

## What Happened

Every time Unity was opened, the console was flooded with errors:

```
[Error] An error occurred while resolving packages:
        One or more packages could not be added to the local file system
```

Followed by dozens of CS0234 and CS0246 compilation errors from packages in `Library/PackageCache`:

| Error Code | Missing Type / Namespace | Affected Package |
|------------|--------------------------|------------------|
| CS0234 | `Tilemaps` namespace | `com.unity.2d.psdimporter`, `com.unity.2d.aseprite` |
| CS0246 | `TileTemplate` | `com.unity.2d.psdimporter` |
| CS0246 | `GridBrush` | `com.unity.2d.tilemap.extras` |
| CS0246 | `GridBrushEditor` | `com.unity.2d.tilemap.extras` |

These errors made it impossible for Unity to compile any editor scripts that depended on the 2D Tilemap system.

---

## Why It Happened

There were **two separate causes** working together to produce these errors.

### Cause 1: Version Mismatches in manifest.json

Three packages were requested at versions **higher than what Unity 6000.1.9f1 ships**:

| Package | Version in `manifest.json` | Actual Builtin Version (6000.1.9f1) |
|---------|---------------------------|--------------------------------------|
| `com.unity.render-pipelines.universal` | `17.3.0` | `17.1.0` |
| `com.unity.multiplayer.center` | `1.0.1` | `1.0.0` |
| `com.unity.test-framework` | `1.6.0` | `1.5.1` |

When Unity's Package Manager tried to resolve these, it couldn't find the requested versions (they don't exist for this editor version). This caused the overall package resolution to fail, which in turn prevented the **`com.unity.2d.tilemap`** builtin package from loading. Since `psdimporter`, `tilemap.extras`, and `aseprite` all depend on `com.unity.2d.tilemap`, they all threw compilation errors about missing Tilemap types.

#### How did wrong versions get into the manifest?

This typically happens when:
1. **Someone opens the project in a newer Unity version** -- the newer editor writes its higher builtin versions into `manifest.json`, then when opened in the older editor those versions don't exist.
2. **Manual editing of `manifest.json`** -- someone types in a version number that doesn't match the editor.
3. **Merging branches** where contributors are on different Unity versions -- the merge takes the "newer" version strings but the editor can't satisfy them.

### Cause 2: EBUSY File Locks from Cursor/VS Code

Even after fixing the version mismatches, Unity showed a more specific error:

```
EBUSY: resource busy or locked, open 'C:\...\Library\PackageCache\.tmp-...\package\Editor\...'
```

This means **another process was holding file locks** on files inside `Library/PackageCache` while Unity was trying to extract packages there. The culprit was **Cursor (VS Code)** -- specifically its **file watcher**.

By default, Cursor/VS Code monitors the entire workspace for file changes (to update IntelliSense, the file explorer, search indexes, etc.). When Unity tries to extract thousands of files into `Library/PackageCache`, Cursor's file watcher opens those same files for indexing, creating **EBUSY (resource busy)** conflicts on Windows. This is a Windows-specific issue because Windows enforces mandatory file locks, unlike macOS/Linux.

The fix was adding `files.watcherExclude` to `.vscode/settings.json` to tell Cursor to completely ignore the `Library/` folder (and other Unity-generated folders). The `files.exclude` setting that was already there only hides files from the sidebar -- it does **not** stop the file watcher.

---

## How Builtin Packages Work in Unity 6

In Unity 6 (6000.x), many packages that used to live on the Unity Package Registry were converted to **builtin packages**. This is an important distinction:

| Package Type | Where It Lives | Can You Change the Version? |
|-------------|----------------|---------------------------|
| **Registry** | Downloaded to `Library/PackageCache` from `packages.unity.com` | Yes -- specify any published version in `manifest.json` |
| **Builtin** | Baked into the Unity Editor installation folder | **No** -- the version is fixed to whatever the editor ships |
| **Module** | Core engine module inside the editor | **No** -- always `1.0.0`, tied to the engine |

**Key packages that are now builtin in Unity 6 (do NOT manually change their versions):**

- `com.unity.render-pipelines.universal` (URP)
- `com.unity.render-pipelines.core`
- `com.unity.shadergraph`
- `com.unity.multiplayer.center`
- `com.unity.test-framework`
- `com.unity.ugui`
- `com.unity.2d.tilemap`
- `com.unity.2d.sprite`
- `com.unity.feature.2d`

If you request a version higher than what's bundled, the Package Manager will fail to resolve it because it can't download builtin packages from the registry.

---

## The Fixes That Were Applied

### Fix 1: Corrected version mismatches in manifest.json

Three version numbers in `Packages/manifest.json` were corrected to match Unity 6000.1.9f1's builtin versions:

```diff
- "com.unity.multiplayer.center": "1.0.1",
+ "com.unity.multiplayer.center": "1.0.0",

- "com.unity.render-pipelines.universal": "17.3.0",
+ "com.unity.render-pipelines.universal": "17.1.0",

- "com.unity.test-framework": "1.6.0",
+ "com.unity.test-framework": "1.5.1",
```

Additionally, `Library/PackageCache` was deleted to force a clean re-download, and `packages-lock.json` was deleted to force a clean re-resolution.

### Fix 2: Added file watcher exclusions to .vscode/settings.json

Two new settings blocks were added to `.vscode/settings.json`:

**`files.watcherExclude`** -- Stops Cursor's file watcher from monitoring Unity-generated folders:
```json
"files.watcherExclude": {
    "**/Library/**": true,
    "**/Temp/**": true,
    "**/Obj/**": true,
    "**/Build/**": true,
    "**/Builds/**": true,
    "**/Logs/**": true,
    "**/UserSettings/**": true,
    "**/MemoryCaptures/**": true
}
```

**`search.exclude`** -- Prevents Ctrl+Shift+F searches from scanning these folders:
```json
"search.exclude": {
    "**/Library/**": true,
    "**/Temp/**": true,
    "**/Obj/**": true,
    "**/Build/**": true,
    "**/Logs/**": true
}
```

These are **different from `files.exclude`** (which was already set). `files.exclude` only hides files from the sidebar. `files.watcherExclude` stops the background file monitoring that causes EBUSY locks. `search.exclude` prevents unnecessary indexing of generated files.

---

## Rules to Prevent This From Happening Again

### 1. Everyone MUST use the same Unity Editor version
- The project uses **Unity 6000.1.9f1** (`ed7b183fd33d`).
- Verify in `ProjectSettings/ProjectVersion.txt` or via Unity Hub.
- If Unity Hub prompts you to upgrade the project, **decline**.

### 2. Never manually edit `Packages/manifest.json` version numbers
- Use the **Unity Package Manager window** (Window > Package Manager) to add, remove, or update packages.
- The Package Manager knows which versions are valid for your editor.

### 3. Be careful when merging branches
- If a merge conflict appears in `manifest.json` or `packages-lock.json`, **always keep the versions that match your editor**.
- When in doubt, keep the **lower** version number for builtin packages -- higher versions likely came from a newer editor.

### 4. Never commit `Library/` to Git
- The `Library/` folder (including `PackageCache`) is local-only and regenerated by Unity.
- It is already in `.gitignore` -- make sure it stays there.

### 5. Keep .vscode/settings.json watcher exclusions in place
- The project includes `files.watcherExclude` in `.vscode/settings.json` that prevents Cursor/VS Code from locking files inside `Library/`.
- **Do not remove these exclusions.** Without them, Cursor's file watcher will lock files that Unity needs to write, causing EBUSY errors on Windows.
- If you clone the repo fresh and still get EBUSY errors, verify `.vscode/settings.json` was pulled correctly.

### 6. If you add a new package, verify it compiles before pushing
- Open Unity, wait for compilation to finish, and confirm zero errors in the Console before committing `manifest.json` and `packages-lock.json`.

---

## Recovery Steps If It Happens Again

If you or a team member sees the "One or more packages could not be added to the local file system" error:

### Quick Fix (try this first)
1. **Close Unity completely**
2. **Delete** `Library/PackageCache/` folder
3. **Delete** `Packages/packages-lock.json`
4. **Reopen Unity** -- wait for it to re-resolve all packages (~1-2 minutes)

### Full Fix (if quick fix doesn't work)
1. **Close Unity completely**
2. **Check `Packages/manifest.json`** for version mismatches:
   - Open the file and look for any builtin package with a version higher than expected
   - Cross-reference with `ProjectSettings/ProjectVersion.txt` to know your editor version
   - Correct any wrong versions (see the table in [Why It Happened](#why-it-happened))
3. **Delete** the entire `Library/` folder (it will be fully regenerated)
4. **Delete** `Packages/packages-lock.json`
5. **Reopen Unity** and wait for full reimport (~5-10 minutes for a clean Library rebuild)

### EBUSY Fix (if you see "resource busy or locked")
1. **Close Cursor/VS Code completely** (this is the process holding the locks)
2. **Close Unity completely**
3. **Delete** `Library/PackageCache/` folder
4. **Open Unity first** (let it finish resolving packages)
5. **Then open Cursor** after Unity has fully loaded
6. Verify `.vscode/settings.json` has the `files.watcherExclude` entries for `**/Library/**`

### Nuclear Option (if nothing else works)
1. Close both Unity and Cursor
2. `git checkout -- Packages/manifest.json` to restore the last known-good manifest
3. Delete `Library/` and `Packages/packages-lock.json`
4. Open Unity first, let it fully import, then open Cursor

---

## File Locations

| File / Asset | Path |
|--------------|------|
| Package Manifest | `Packages/manifest.json` |
| Package Lock File | `Packages/packages-lock.json` |
| Project Version | `ProjectSettings/ProjectVersion.txt` |
| VS Code / Cursor Workspace Settings | `.vscode/settings.json` |
| Git Ignore | `.gitignore` |
| Package Cache (local, not committed) | `Library/PackageCache/` |
| This Guide | `docs/2025-02-17_unity-package-resolution-guide.md` |