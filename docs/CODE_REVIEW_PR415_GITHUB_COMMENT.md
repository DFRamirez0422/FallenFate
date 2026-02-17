Code review for **My Addons for room2 and fixes (PR #415)**. Please address when you can—thanks for the Room2 work.

---

## What's working well

- **CollidableObject base class** – Shared interaction pattern (collision → OnCollide) is a good idea and keeps behavior consistent.
- **ScriptableObject for items** – Using `Item_Data` for keys/mementos is appropriate and makes design data reusable.
- **Header / Tooltip** – Use of `[Header]` and `[Tooltip]` in several scripts helps Inspector readability.
- **Separation of concerns** – Doors, generators, pickups, and cutscene are split into separate scripts.

The features are in and usable. The list below is to make the code more robust, performant, and aligned with Unity best practices so it's easier to maintain and extend.

---

## Critical: fix these first

### 1. **CollidableObject – Update runs every frame and calls OnCollide for every overlap**

**File:** `CollidableObject.cs`

- `Update()` runs **every frame** and calls `OnCollide()` for **every** overlapping collider (including non-player: ground, walls, etc.).
- That means every door, generator, and pickup does Overlap + loop + virtual call every frame, and the base `OnCollide` can spam `Debug.Log`.

**Unity guidance:** Minimize work in `Update`; avoid per-frame work that scales with many objects.

**Change:**

- Only call `OnCollide` for objects you care about (e.g. player). Use `ContactFilter2D` to restrict to a "Player" layer, or check `other.CompareTag("Player")` inside the loop and skip non-player.
- Optionally use **trigger callbacks** (`OnTriggerEnter2D` / `OnTriggerStay2D` / `OnTriggerExit2D`) instead of manual Overlap in `Update`, so Unity only invokes when something enters/stays. That's usually cheaper and clearer.

### 2. **PickUpObjects – Wrong reference in OnTriggerExit2D (bug)**

**File:** `PickUpObjects.cs` (lines 91–97)

- You instantiate the prompt into `PickUpPromptPrefab`, but in `OnTriggerExit2D` you call `PickUpPrompt.SetActive(false)` and clear text on `PickUpPrompt` (the **prefab** from Resources), then `Destroy(PickUpPromptPrefab)`.
- So you're modifying the wrong object and the prompt instance may not behave correctly.

**Change:** Use one name for the **spawned instance** (e.g. `_spawnedPrompt`) and use that for SetActive, text, and Destroy. Never use the prefab reference for the live instance.

### 3. **Room2_CutScene_Player – Prefab reference overwritten + coroutine type**

**File:** `Room2_CutScene_Player.cs`

- `CutsceneCanvas = Instantiate(CutsceneCanvas)` overwrites the prefab reference with the instance. The next time the trigger runs (e.g. after a reload), you'd instantiate an instance instead of the prefab.
- `IEnumerator<WaitForSeconds>` is not the correct return type for a coroutine; use `IEnumerator` and `yield return new WaitForSeconds(frameDuration)` inside it.

**Change:** Store the instance in a separate variable (e.g. `_cutsceneInstance`) and keep `CutsceneCanvas` as the prefab. Declare the coroutine as `IEnumerator` and use `yield return new WaitForSeconds(frameDuration)`.

### 4. **Item_Data – ScriptableObject with mutable `collected`**

**File:** `Item_Data.cs`

- `collected` is a field on a **ScriptableObject**. SOs are assets; changing them at runtime changes the asset. If the same `Item_Data` asset is used in multiple places (e.g. one key type for several doors), they all share one `collected` value, which can cause subtle bugs and makes save/load and scene reload tricky.

**Change:** Keep "collected" state in runtime-only data (e.g. in `PickUp_Manager`: a `HashSet` or list of "collected" item IDs or names), not on the ScriptableObject. Use `Item_Data` for design data (name, type, prefab) only.

---

## Performance and Unity best practices

### 5. **`GetComponentsInChildren<Text>()` called repeatedly**

**Files:** `Activate_Generators.cs`, `OpenDoors.cs`, `PickUpObjects.cs`, `Powered_Door.cs`, `OpenDoor_NoKey.cs`

- Every time you set prompt text you do `GetComponentsInChildren<Text>()[0]`, etc. These calls allocate and traverse the hierarchy.

**Change:** Cache the Text references once when you instantiate the prompt (e.g. in a small helper or when you create the prompt), then reuse those references for all text updates.

### 6. **GameObject.Find / FindGameObjectWithTag in Start/Awake**

**Files:** `OpenDoors.cs`, `PickUpObjects.cs`, `Room2_CutScene_Player.cs`

- `GameObject.Find("Item_PickUp_Manager")` and `GameObject.FindGameObjectWithTag("Player")` tie the script to exact names/tags and scene hierarchy. Finds are also relatively expensive.

**Change:** Prefer **serialized references**: `[SerializeField] private PickUp_Manager pickUpManager` and assign in the Inspector (or use a small bootstrap/setup that assigns it). For the player, use `[SerializeField] private PlayerMovement playerMovement` or a well-known singleton if the project already uses one.

### 7. **Asset paths: use SerializeField (or public) references instead of string paths**

**Why:** In Unity, when you assign an asset to a `[SerializeField]` or `public` field that derives from `UnityEngine.Object` (e.g. `GameObject`, `ScriptableObject`, prefab), Unity **does not** store a path string. It stores a **GUID** (for the asset file) and a **fileID** (for the object inside that file). The Editor uses these to keep the reference correct when you rename, move, or refactor assets. String paths do not get that tracking—they break on renames/moves and don't show up in the dependency graph or build correctly.

**Reference:** [How Unity uses serialization](https://docs.unity3d.com/Manual/script-serialization-how-unity-uses.html), [Direct references and GUIDs](https://docs.unity3d.com/Manual/assets-direct-reference.html).

**Change:** Do **not** use `Resources.Load("...")` or a `string scriptableObjectPath` for assets you can assign in the Editor. Use a **serialized reference** instead:

- `[SerializeField] private GameObject actionDescriptionPrefab;` — assign the prefab in the Inspector; Unity tracks it by GUID.
- `[SerializeField] private Item_Data itemData;` — assign the ScriptableObject asset; no path or Resources.Load needed.

Then remove the `Resources.Load` / path-based loading and assign the exact asset in the Inspector. Unity will keep track via GUIDs.

**Exact paths to replace (assign these assets via Inspector instead):**

| Script | Current usage | Replace with |
|--------|----------------|--------------|
| `Activate_Generators.cs` | `Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription")` | `[SerializeField] private GameObject activateGeneratorPrompt;` – assign **ActionDescription** prefab |
| `OpenDoors.cs` | `Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription")` | `[SerializeField] private GameObject openDoorPrompt;` – assign **ActionDescription** prefab |
| `OpenDoor_NoKey.cs` | `Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription")` | `[SerializeField] private GameObject openDoorPrompt;` – assign **ActionDescription** prefab |
| `Powered_Door.cs` | `Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription")` | `[SerializeField] private GameObject poweredDoorPrompt;` – assign **ActionDescription** prefab |
| `PickUpObjects.cs` | `Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription")` | `[SerializeField] private GameObject pickUpPrompt;` – assign **ActionDescription** prefab |
| `PickUpObjects.cs` | `Resources.Load<Item_Data>(scriptableObjectPath)` + string field | `[SerializeField] private Item_Data itemData;` – assign **Item_Data** asset directly; remove `scriptableObjectPath` |
| `Room2_CutScene_Player.cs` | `Resources.Load<GameObject>("Prefabs/Room2_CutScenePrefabs/Room2CutScene")` | `[SerializeField] private GameObject cutsceneCanvas;` – assign **Room2CutScene** prefab |

After this change, **GUIDs** in the scene/prefab files keep track of the assets; you can rename or move them in the Project window and references stay valid. No hardcoded paths.

### 8. **Powered_Door – Polling two generators every frame in Update**

**File:** `Powered_Door.cs`

- You check `activateGenerators.Activate_Generator && activate_Generator2.Activate_Generator` in `Update()` every frame.

**Change:** Make this event-driven: e.g. when a generator is activated, it notifies a simple "Room2Power" or "Powered_Door" manager, which updates state and enables doors. Then the door only checks a single "isPowered" flag when the player interacts, or the manager enables the door when both are on. Avoids per-frame polling.

### 9. **GetComponent in one-off methods**

**Files:** `Powered_Door.cs`, `OpenDoor_NoKey.cs` – `GetComponent<Collider2D>()` in `OpenDoor()`.

- GetComponent has a cost; doing it every time the door opens is unnecessary.

**Change:** Cache the `Collider2D` in `Start()` or `Awake()` (like `CollidableObject` does for its collider) and reuse the cached reference.

### 10. **transform.Translate for "opening" doors**

**Files:** `Powered_Door.cs`, `OpenDoor_NoKey.cs`

- `transform.Translate(-1f, 0, 0)` (or with `_TranslatePosition`) is world-space by default and doesn't scale well (different door sizes/orientations). It also doesn't animate.

**Change:** Prefer an **Animator** (like in `OpenDoors.cs`) for open/close, or at least use `Translate(..., Space.Self)` and consider storing open/closed state so you can support closing again later.

---

## Code quality and safety

### 11. **Unused / unnecessary namespaces**

**Files:**

- `Activate_Generators.cs`: `Unity.VisualScripting`
- `PickUpObjects.cs`: `System.Linq.Expressions`, `System.Net.NetworkInformation`, `Unity.VisualScripting`
- `Powered_Door.cs`: `System.Runtime.CompilerServices`
- `PickUp_Manager.cs`: `System.Net.Http.Headers`

**Change:** Remove unused `using` directives to avoid confusion and keep compilation clean.

### 12. **Mixing 2D and 3D physics**

**File:** `Activate_Generators.cs`

- You use `OnCollisionEnter2D` / `OnCollisionExit2D` (2D) and `OnCollisionStay` (3D). In a 2D project, the 3D callback won't run.

**Change:** Use only 2D callbacks: `OnCollisionEnter2D`, `OnCollisionStay2D`, `OnCollisionExit2D`, and implement the "stay" logic in the 2D version so behavior is consistent.

### 13. **PickUpObjects – Destroy(this.gameObject) vs Destroy(gameObject)**

**File:** `PickUpObjects.cs`

- `var copy = this.gameObject; Destroy(copy);` is the same as `Destroy(gameObject);`. The extra variable doesn't add clarity.

**Change:** Use `Destroy(gameObject);` (and same idea in any similar script).

### 14. **PickUp_Manager – Public mutable list**

**File:** `PickUp_Manager.cs`

- `public List<Item_Data> items` lets any script add/remove/clear. That makes it hard to enforce rules (e.g. no duplicates, or "collected" only via one path).

**Change:** Keep a private list and expose read-only access (e.g. `IReadOnlyList` or a method like `bool HasItem(Item_Data item)`). Add/remove only through the manager (e.g. `AddItem(Item_Data item)`), so you can add checks and avoid duplicate or invalid state.

### 15. **Null checks before use**

**Files:** Several scripts use `_SpawnedPrompt` or similar after instantiation. If something destroys the prompt elsewhere, or if Instantiate fails, you can get NullReferenceException.

**Change:** After Instantiate, store in a local or field and null-check before SetActive/Destroy/GetComponent. When hiding/destroying, set the reference to `null` so the next enter doesn't reuse a destroyed object.

---

## Summary checklist for the author

- [ ] **CollidableObject:** Only call OnCollide for player (filter by layer/tag) or switch to trigger callbacks; avoid per-frame Overlap for all overlaps.
- [ ] **PickUpObjects:** Fix prompt reference in Exit (use spawned instance, not prefab); remove unused usings; consider `Destroy(gameObject)`.
- [ ] **Room2_CutScene_Player:** Don't overwrite prefab reference with instance; fix coroutine return type to `IEnumerator` and use `WaitForSeconds`.
- [ ] **Item_Data:** Move "collected" state out of the ScriptableObject into PickUp_Manager (or similar) runtime state.
- [ ] **All prompt UI:** Cache `GetComponentsInChildren<Text>()` when creating the prompt; avoid repeated GetComponent.
- [ ] **Asset paths → SerializeField:** Replace every `Resources.Load(...)` and string path with `[SerializeField]` or public reference; assign assets in the Inspector so Unity tracks them by GUID (see table in section 7 above).
- [ ] **Find / Resources:** Replace GameObject.Find and FindGameObjectWithTag with SerializeField references where possible.
- [ ] **Powered_Door:** Cache Collider2D; consider event-driven "power on" instead of polling in Update.
- [ ] **Doors:** Prefer Animator or cached transform/collider over repeated GetComponent and Translate.
- [ ] **Activate_Generators:** Use only 2D collision callbacks; remove unused using.
- [ ] **PickUp_Manager:** Encapsulate the list (private + add/remove/has methods); remove unused using.

---

## References

- [Unity – Programming best practices](https://docs.unity3d.com/Manual/programming-best-practices.html)
- [Execution order and Update](https://docs.unity3d.com/Manual/execution-order.html)
- [MonoBehaviour lifecycle](https://docs.unity3d.com/Manual/ExecutionOrder.html)
- [ScriptableObjects](https://docs.unity3d.com/Manual/class-ScriptableObject.html)
- [How Unity uses serialization](https://docs.unity3d.com/Manual/script-serialization-how-unity-uses.html)
- [Unity object references (GUID / fileID)](https://docs.unity3d.com/Manual/assets-direct-reference.html)

Thanks again for the Room2 addons—addressing these points will make the codebase more maintainable and performant.
