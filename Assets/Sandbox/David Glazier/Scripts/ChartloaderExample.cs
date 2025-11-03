using System.IO;
using UnityEngine;
using ChartLoader.NET.Utils;
using ChartLoader.NET.Framework;

public class ChartLoaderExample : MonoBehaviour
{
    public static ChartReader chartReader;

    [Header("Prefabs & Spawn")]
    public Transform[] notePrefabs;                 // pool to randomize from
    [SerializeField] private Transform enemyAttackPoint;

    [Header("Lane Spacing")]
    [SerializeField] private float laneSpacing = 1.0f;
    public float LaneSpacing { get => laneSpacing; set => laneSpacing = value; }

    [Header("Projectile Toggle")]
    public bool projectilesEnabled = true;
    public KeyCode toggleKey = KeyCode.T;

    [Header("Song Path (Inspector / Runtime)")]
    public string songFolder = "Assets/ChartLoader/ChartLoader/Songs/DOOM";
    public string chartFileName = "notes.chart";
    public string chartSection = "ExpertSingle";
    public KeyCode reloadKey = KeyCode.R;

    // ==== SPAWN DIRECTION ====
    public enum SpawnDirectionMode { X, Z, Angle, FollowPlayer, ContinuousFollowPlayer, Rotating360 }
    [Header("Spawn Direction")]
    public SpawnDirectionMode spawnMode = SpawnDirectionMode.Z;

    [Tooltip("Used only when Spawn Mode = Angle. Degrees in the XZ plane (0 = +Z, 90 = +X).")]
    [Range(0f, 360f)] public float spawnAngleDegrees = 0f;

    [Tooltip("If true, the angle is rotated relative to enemyAttackPoint's Y-rotation. If false, it's world-space.")]
    public bool angleRelativeToEnemy = true;

    [Header("Follow Player Mode")]
    [Tooltip("Target for FollowPlayer mode. If empty, will auto-find object tagged 'Player'.")]
    public Transform playerTarget;

    [Tooltip("Offset angle from direct player direction for spawn lanes (degrees).")]
    [Range(-180f, 180f)] public float playerOffsetAngle = 90f; // Default: perpendicular to player

    [Header("Continuous Follow Player Mode")]
    [Tooltip("How smoothly the direction updates to follow player (higher = more responsive).")]
    [Range(0.1f, 10f)] public float continuousFollowSpeed = 2f;

    [Tooltip("Update rate for continuous following (times per second). Higher = smoother but more expensive.")]
    [Range(1f, 60f)] public float continuousUpdateRate = 10f;

    [Header("360 Rotating Mode")]
    [Tooltip("Rotation speed for 360 mode (degrees per second).")]
    public float rotation360Speed = 30f;

    [Tooltip("Starting angle for 360 rotation (degrees).")]
    [Range(0f, 360f)] public float rotation360StartAngle = 0f;

    [Tooltip("Visualize lanes in Scene view when selected.")]
    public bool drawLaneGizmos = true;

    // ==== NOTE MOVEMENT DIRECTION ====
    [Header("Note Movement Direction")]
    [Tooltip("Direction notes should move after spawning. Usually opposite to spawn direction.")]
    public SpawnDirectionMode movementMode = SpawnDirectionMode.Z;

    [Tooltip("Used only when Movement Mode = Angle. Degrees in the XZ plane (0 = +Z, 90 = +X).")]
    [Range(0f, 360f)] public float movementAngleDegrees = 180f; // Default: move toward player

    [Tooltip("If true, movement angle is rotated relative to enemyAttackPoint's Y-rotation. If false, it's world-space.")]
    public bool movementAngleRelativeToEnemy = true;

    [Header("Movement Follow Player Mode")]
    [Tooltip("Offset angle from direct player direction for movement (degrees). 0 = directly toward player.")]
    [Range(-180f, 180f)] public float movementPlayerOffsetAngle = 0f; // Default: directly toward player

    [Tooltip("For continuous movement following - how often to update direction (times per second).")]
    [Range(1f, 60f)] public float continuousMovementUpdateRate = 15f;

    [Header("Movement Direction Control")]
    [Tooltip("Multiplier for movement direction. Use -1 to reverse direction (toward player instead of away).")]
    public float movementDirectionMultiplier = -1f; // Default: move toward player/enemy

    [Header("Despawn Settings")]
    [Tooltip("Time in seconds before spawned notes are destroyed. Set to 0 to disable auto-despawn.")]
    [SerializeField] private float noteDespawnTime = 10f;

    // Chart timing
    private Note[] _notes = new Note[0];
    private int _nextNoteIdx = 0;
    private float _songStartTime;
    private float _rotation360CurrentAngle; // Current angle for 360 rotation mode
    
    // Despawn tracking
    private System.Collections.Generic.List<NoteDespawnInfo> _activeNotes = new System.Collections.Generic.List<NoteDespawnInfo>();
    
    private class NoteDespawnInfo
    {
        public GameObject noteObject;
        public float spawnTime;
        
        public NoteDespawnInfo(GameObject obj, float time)
        {
            noteObject = obj;
            spawnTime = time;
        }
    }
    
    // Continuous following
    private Vector3 _cachedPlayerDirection = Vector3.forward;
    private Vector3 _cachedMovementDirection = Vector3.forward;
    private float _lastContinuousUpdateTime = 0f;
    private float _lastMovementUpdateTime = 0f;

    void Start()
    {
        chartReader = new ChartReader();
        
        // Auto-find player if not set
        if (playerTarget == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj) playerTarget = playerObj.transform;
        }
        
        // Initialize 360 rotation
        _rotation360CurrentAngle = rotation360StartAngle;
        
        LoadChartAndReset(songFolder, chartFileName, chartSection);
    }

    /// <summary>
    /// Updates cached directions for continuous following modes at specified intervals
    /// </summary>
    private void UpdateContinuousFollowing()
    {
        // Update spawn direction for ContinuousFollowPlayer
        if (spawnMode == SpawnDirectionMode.ContinuousFollowPlayer)
        {
            float timeSinceLastUpdate = Time.time - _lastContinuousUpdateTime;
            if (timeSinceLastUpdate >= (1f / continuousUpdateRate))
            {
                Vector3 targetDirection = CalculatePlayerDirection(playerOffsetAngle);
                _cachedPlayerDirection = Vector3.Slerp(_cachedPlayerDirection, targetDirection, 
                    continuousFollowSpeed * timeSinceLastUpdate);
                _lastContinuousUpdateTime = Time.time;
            }
        }

        // Update movement direction for ContinuousFollowPlayer
        if (movementMode == SpawnDirectionMode.ContinuousFollowPlayer)
        {
            float timeSinceLastMovementUpdate = Time.time - _lastMovementUpdateTime;
            if (timeSinceLastMovementUpdate >= (1f / continuousMovementUpdateRate))
            {
                Vector3 targetMovementDirection = CalculatePlayerDirection(movementPlayerOffsetAngle);
                _cachedMovementDirection = Vector3.Slerp(_cachedMovementDirection, targetMovementDirection, 
                    continuousFollowSpeed * timeSinceLastMovementUpdate);
                _lastMovementUpdateTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Calculates direction to player with optional angle offset
    /// </summary>
    private Vector3 CalculatePlayerDirection(float offsetAngle)
    {
        if (playerTarget == null || enemyAttackPoint == null) return Vector3.forward;
        
        // Get direction to player
        Vector3 toPlayer = (playerTarget.position - enemyAttackPoint.position);
        toPlayer.y = 0f; // Keep in XZ plane
        
        if (toPlayer.sqrMagnitude < 0.0001f) return Vector3.forward; // Avoid division by zero
        
        // Apply offset angle
        float angleToPlayer = Mathf.Atan2(toPlayer.x, toPlayer.z) * Mathf.Rad2Deg;
        float finalAngle = angleToPlayer + offsetAngle;
        
        Vector3 direction = Quaternion.Euler(0f, finalAngle, 0f) * Vector3.forward;
        direction.y = 0f;
        return direction.normalized;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) projectilesEnabled = !projectilesEnabled;
        if (Input.GetKeyDown(reloadKey)) LoadChartAndReset(songFolder, chartFileName, chartSection);

        // Update 360 rotation
        if (spawnMode == SpawnDirectionMode.Rotating360 || movementMode == SpawnDirectionMode.Rotating360)
        {
            _rotation360CurrentAngle += rotation360Speed * Time.deltaTime;
            _rotation360CurrentAngle = _rotation360CurrentAngle % 360f; // Keep in 0-360 range
        }

        // Update continuous following directions
        UpdateContinuousFollowing();

        float elapsed = Time.time - _songStartTime;

        while (_nextNoteIdx < _notes.Length && _notes[_nextNoteIdx].Seconds <= elapsed)
        {
            if (projectilesEnabled)
                SpawnNoteImmediate(_notes[_nextNoteIdx]);

            _nextNoteIdx++;
        }
        
        // Handle despawn logic
        if (noteDespawnTime > 0f)
        {
            UpdateDespawnNotes();
        }
    }
    
    /// <summary>
    /// Updates and destroys notes that have exceeded their despawn time
    /// </summary>
    private void UpdateDespawnNotes()
    {
        float currentTime = Time.time;
        
        // Iterate backwards so we can safely remove items while iterating
        for (int i = _activeNotes.Count - 1; i >= 0; i--)
        {
            NoteDespawnInfo info = _activeNotes[i];
            
            // Check if note was already destroyed externally
            if (info.noteObject == null)
            {
                _activeNotes.RemoveAt(i);
                continue;
            }
            
            // Check if despawn time has elapsed
            if (currentTime - info.spawnTime >= noteDespawnTime)
            {
                Destroy(info.noteObject);
                _activeNotes.RemoveAt(i);
            }
        }
    }

    public void SetSongPath(string folder, string fileName, string section = "ExpertSingle")
    {
        songFolder = folder;
        chartFileName = fileName;
        chartSection = string.IsNullOrEmpty(section) ? "ExpertSingle" : section;
        LoadChartAndReset(songFolder, chartFileName, chartSection);
    }

    private void LoadChartAndReset(string folder, string fileName, string section)
    {
        string fullPath = BuildFullChartPath(folder, fileName);
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[Spawner] Chart not found: {fullPath}");
            _notes = new Note[0];
            _nextNoteIdx = 0;
            _songStartTime = Time.time;
            return;
        }

        Chart chart = chartReader.ReadChartFile(fullPath);
        Note[] notes = chart?.GetNotes(section);

        if (notes == null || notes.Length == 0)
        {
            Debug.LogError($"[Spawner] No notes for section '{section}' in {fullPath}");
            _notes = new Note[0];
        }
        else
        {
            _notes = notes;
            System.Array.Sort(_notes, (a, b) => a.Seconds.CompareTo(b.Seconds));
        }

        _nextNoteIdx = 0;
        _songStartTime = Time.time;
        
        // Clear existing notes when reloading chart
        ClearAllActiveNotes();
        
        Debug.Log($"[Spawner] Loaded: {fullPath} (section: {section}), notes: {_notes.Length}");
    }

    private string BuildFullChartPath(string folder, string fileName)
    {
        bool isAbsolute = Path.IsPathRooted(folder);
        string basePath = isAbsolute ? folder : Path.Combine(Application.dataPath.Replace("/Assets", ""), folder);
        return Path.Combine(basePath, fileName);
    }
    
    /// <summary>
    /// Clears all active spawned notes (useful when reloading charts or resetting)
    /// </summary>
    private void ClearAllActiveNotes()
    {
        foreach (var info in _activeNotes)
        {
            if (info.noteObject != null)
            {
                Destroy(info.noteObject);
            }
        }
        _activeNotes.Clear();
    }

    // === Randomized spawn per note ===
    private void SpawnNoteImmediate(Note note)
    {
        if (notePrefabs == null || notePrefabs.Length == 0 || note.ButtonIndexes == null || enemyAttackPoint == null)
            return;

        int lanes = notePrefabs.Length;
        float half = (lanes - 1) / 2f;

        // ----- SPAWN DIRECTION: compute the direction in XZ plane -----
        Vector3 laneDir = GetLaneDirection();              // unit vector in XZ plane
        Vector3 movementDir = GetMovementDirection();      // direction notes should move
        Vector3 origin = enemyAttackPoint.position;

        for (int i = 0; i < lanes; i++)
        {
            if (i < note.ButtonIndexes.Length && note.ButtonIndexes[i])
            {
                float distance = (i - half) * laneSpacing;
                Vector3 spawnPosition = origin + laneDir * distance;
                
                // Apply direction multiplier to movement direction
                Vector3 finalMovementDir = movementDir * movementDirectionMultiplier;
                SpawnRandomPrefab(spawnPosition, finalMovementDir);
            }
        }
    }

    // Picks a random prefab from notePrefabs and spawns it at 'point', then injects movement direction
    private void SpawnRandomPrefab(Vector3 point, Vector3 movementDirection)
    {
        if (notePrefabs == null || notePrefabs.Length == 0) return;

        int index = Random.Range(0, notePrefabs.Length);
        Transform prefab = notePrefabs[index];
        if (prefab == null) return;

        // World-space instantiate (avoid inheriting parent rotation/scale)
        GameObject spawnedNote = Instantiate(prefab, point, Quaternion.identity).gameObject;

        // Inject movement direction into PerObjectMover component
        PerObjectMover mover = spawnedNote.GetComponent<PerObjectMover>();
        if (mover != null)
        {
            mover.SetInjectedDirection(movementDirection);
        }
        
        // Track note for despawn if despawn is enabled
        if (noteDespawnTime > 0f)
        {
            _activeNotes.Add(new NoteDespawnInfo(spawnedNote, Time.time));
        }
    }

    // ===== Helpers =====

    /// <summary>
    /// Returns a unit vector in the XZ plane along which lanes are spread.
    /// X  => Vector3.right
    /// Z  => Vector3.forward
    /// Angle => rotated forward by spawnAngleDegrees (optionally relative to enemyAttackPoint)
    /// FollowPlayer => perpendicular to player direction (with offset) - calculated each spawn
    /// ContinuousFollowPlayer => smoothly follows player using cached direction
    /// Rotating360 => continuously rotating direction
    /// 0° is +Z, 90° is +X, 180° is -Z, etc.
    /// </summary>
    private Vector3 GetLaneDirection()
    {
        switch (spawnMode)
        {
            case SpawnDirectionMode.X:
                return Vector3.right;

            case SpawnDirectionMode.Z:
                return Vector3.forward;

            case SpawnDirectionMode.Angle:
                float baseYaw = angleRelativeToEnemy ? enemyAttackPoint.eulerAngles.y : 0f;
                float yaw = baseYaw + spawnAngleDegrees;
                // Rotate +Z by yaw around Y
                Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
                dir.y = 0f;
                return dir.normalized;

            case SpawnDirectionMode.FollowPlayer:
                return CalculatePlayerDirection(playerOffsetAngle);

            case SpawnDirectionMode.ContinuousFollowPlayer:
                return _cachedPlayerDirection.normalized;

            case SpawnDirectionMode.Rotating360:
                Vector3 rotDir = Quaternion.Euler(0f, _rotation360CurrentAngle, 0f) * Vector3.forward;
                rotDir.y = 0f;
                return rotDir.normalized;

            default:
                return Vector3.forward;
        }
    }

    /// <summary>
    /// Returns a unit vector in the XZ plane along which notes should move after spawning.
    /// This is separate from the lane direction to allow for diagonal movement patterns.
    /// FollowPlayer => directly toward player (with optional offset) - calculated each spawn
    /// ContinuousFollowPlayer => smoothly follows player using cached direction  
    /// Rotating360 => continuously rotating direction (independent of spawn rotation)
    /// </summary>
    private Vector3 GetMovementDirection()
    {
        switch (movementMode)
        {
            case SpawnDirectionMode.X:
                return Vector3.right;

            case SpawnDirectionMode.Z:
                return Vector3.forward;

            case SpawnDirectionMode.Angle:
                float baseYaw = movementAngleRelativeToEnemy ? enemyAttackPoint.eulerAngles.y : 0f;
                float yaw = baseYaw + movementAngleDegrees;
                // Rotate +Z by yaw around Y
                Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
                dir.y = 0f;
                return dir.normalized;

            case SpawnDirectionMode.FollowPlayer:
                return CalculatePlayerDirection(movementPlayerOffsetAngle);

            case SpawnDirectionMode.ContinuousFollowPlayer:
                return _cachedMovementDirection.normalized;

            case SpawnDirectionMode.Rotating360:
                // Use current rotation angle for movement (could be different from spawn if desired)
                Vector3 rotMoveDir = Quaternion.Euler(0f, _rotation360CurrentAngle, 0f) * Vector3.forward;
                rotMoveDir.y = 0f;
                return rotMoveDir.normalized;

            default:
                return Vector3.forward;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawLaneGizmos || enemyAttackPoint == null || notePrefabs == null) return;

        int lanes = Mathf.Max(1, notePrefabs.Length);
        float half = (lanes - 1) / 2f;
        Vector3 origin = enemyAttackPoint.position;
        Vector3 laneDir = GetLaneDirection();
        Vector3 movementDir = GetMovementDirection();
        Vector3 rightish = new Vector3(laneDir.z, 0f, -laneDir.x); // perpendicular for tick marks

        // Draw lane positions
        Gizmos.color = Color.cyan;
        for (int i = 0; i < lanes; i++)
        {
            float distance = (i - half) * laneSpacing;
            Vector3 p = origin + laneDir * distance;
            Gizmos.DrawSphere(p, 0.05f);
            // small tick to show perpendicular
            Gizmos.DrawLine(p - rightish * 0.1f, p + rightish * 0.1f);
            
            // Draw movement direction from each spawn point
            Gizmos.color = Color.red;
            Vector3 finalMovementDir = movementDir * movementDirectionMultiplier;
            Gizmos.DrawRay(p, finalMovementDir * -1.5f);
            Gizmos.color = Color.cyan;
        }

        // Spawn direction arrow
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + laneDir * laneSpacing * Mathf.Max(2, lanes));

        // Movement direction arrow from origin
        Gizmos.color = Color.green;
        Vector3 finalMovementDirForGizmo = movementDir * movementDirectionMultiplier;
        Gizmos.DrawLine(origin, origin + finalMovementDirForGizmo * 2f);

        // Draw player connection if in FollowPlayer modes
        if ((spawnMode == SpawnDirectionMode.FollowPlayer || spawnMode == SpawnDirectionMode.ContinuousFollowPlayer ||
             movementMode == SpawnDirectionMode.FollowPlayer || movementMode == SpawnDirectionMode.ContinuousFollowPlayer) 
            && playerTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(origin, playerTarget.position);
            Gizmos.DrawWireSphere(playerTarget.position, 0.2f);
            
            // Show continuous following cache directions
            if (spawnMode == SpawnDirectionMode.ContinuousFollowPlayer)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(origin, _cachedPlayerDirection * 3f);
            }
            if (movementMode == SpawnDirectionMode.ContinuousFollowPlayer)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin + Vector3.up * 0.1f, _cachedMovementDirection * 2.5f);
            }
        }
    }
#endif
}