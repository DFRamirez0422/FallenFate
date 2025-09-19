using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChartLoader.NET.Framework;
using ChartLoader.NET.Utils;

public class SimpleNoteSpawner : MonoBehaviour
{
    [Header("Chart File")]
    public string songFolder = "Assets/ChartLoader/ChartLoader/Songs/DOOM";
    public string chartFileName = "notes.chart";
    public string chartSection = "ExpertSingle";

    [Header("Lane Setup")]
    public Transform[] lanePositions = new Transform[5]; // 5 lane positions
    public GameObject notePrefab;
    
    [Header("Mode")]
    public bool useSingleLane = false;
    [Range(0, 4)] public int singleLaneIndex = 2; // Middle lane
    
    [Header("Spawning")]
    public float noteSpacing = 100f; // Distance between notes (UI units)  
    public bool useXAxis = true; // True for X-axis, False for Y-axis
    
    [Header("Hit Target")]
    public Transform hitTarget; // Where notes need to reach to be hittable
    public float hitWindow = 0.2f; // Time window for successful hits
    
    [Header("Timing")]
    public float travelTime = 2f; // How long notes take to reach hit target
    public bool autoStart = true; // Auto start the song
    
    private float songStartTime;
    private bool songRunning;
    private int score = 0;
    private List<MovingNote> activeNotes = new List<MovingNote>();
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool showTimingDebug = false; // Extra timing visualization

    void Start()
    {
        if (autoStart)
        {
            StartSong();
        }
    }

    void Update()
    {
        if (!songRunning) return;

        // Show current timing info
        if (showTimingDebug)
        {
            float songTime = Time.time - songStartTime;
            Debug.Log($"[Timing] Song Time: {songTime:F3}s");
        }

        // Handle input
        if (Input.GetMouseButtonDown(0))
        {
            TryHitNote();
        }

        // Update active notes
        UpdateMovingNotes();
        
        // Show rhythm timing debug
        if (showTimingDebug && songRunning)
        {
            float songTime = Time.time - songStartTime;
            
            // Find the next note that should be hit
            var nextNote = activeNotes.Where(n => n.isActive && n.time >= songTime)
                                    .OrderBy(n => n.time)
                                    .FirstOrDefault();
                                    
            if (nextNote.gameObject != null)
            {
                float timeUntilHit = nextNote.time - songTime;
                if (timeUntilHit <= 1f) // Only show when close
                {
                    Debug.Log($"[Rhythm Check] Song: {songTime:F3}s | Next Hit: {nextNote.time:F3}s | In: {timeUntilHit:F3}s");
                }
            }
        }
    }

    [ContextMenu("Start Song")]
    public void StartSong()
    {
        // Clear any existing notes
        ClearExistingNotes();
        
        // Load notes from chart
        var notes = LoadNotesFromChart();
        if (notes == null || notes.Count == 0)
        {
            Debug.LogWarning("[SimpleNoteSpawner] No notes to play!");
            return;
        }

        // Start the song
        songStartTime = Time.time;
        songRunning = true;
        score = 0;
        
        // Prepare notes for spawning
        PrepareNotesForSpawning(notes);
        
        if (showDebugInfo)
            Debug.Log($"[SimpleNoteSpawner] Song started! {notes.Count} notes loaded. Score: {score}");
    }

    private void PrepareNotesForSpawning(List<SimpleNote> notes)
    {
        activeNotes.Clear();
        
        foreach (var note in notes)
        {
            var movingNote = new MovingNote
            {
                time = note.time, // This is when the note should be HIT (on rhythm)
                lane = note.lane,
                spawnTime = note.time - travelTime, // Spawn earlier so it arrives on rhythm
                hasSpawned = false,
                gameObject = null,
                isActive = false
            };
            
            activeNotes.Add(movingNote);
        }
        
        if (showDebugInfo)
            Debug.Log($"[SimpleNoteSpawner] Prepared {activeNotes.Count} notes. First note hits at {activeNotes[0].time:F3}s, spawns at {activeNotes[0].spawnTime:F3}s");
    }

    private void UpdateMovingNotes()
    {
        float currentTime = Time.time - songStartTime;
        
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];
            
            // Spawn note if it's time
            if (!note.hasSpawned && currentTime >= note.spawnTime)
            {
                SpawnNote(ref note, i);
            }
            
            // Move note if it's active
            if (note.isActive && note.gameObject != null)
            {
                UpdateNotePosition(note, currentTime);
                
                // Remove note if it's too late (missed)
                if (currentTime > note.time + hitWindow)
                {
                    if (showDebugInfo)
                        Debug.Log($"[SimpleNoteSpawner] Missed note at time {note.time:F2}s");
                        
                    Destroy(note.gameObject);
                    activeNotes.RemoveAt(i);
                }
            }
        }
    }

    private void SpawnNote(ref MovingNote note, int index)
    {
        if (notePrefab == null || lanePositions[note.lane] == null)
            return;
            
        // Calculate spawn position (where note starts)
        Vector3 spawnPos = CalculateNoteStartPosition(note.lane);
        
        // Create note
        GameObject noteObj = Instantiate(notePrefab, spawnPos, lanePositions[note.lane].rotation);
        noteObj.name = $"Note_Lane{note.lane}_Time{note.time:F2}";
        noteObj.transform.SetParent(transform, false);
        
        // Add note data component
        var noteData = noteObj.GetComponent<MovingNoteData>();
        if (noteData == null)
            noteData = noteObj.AddComponent<MovingNoteData>();
            
        noteData.targetTime = note.time;
        noteData.lane = note.lane;
        noteData.noteIndex = index;
        
        // Update the note in our list
        note.hasSpawned = true;
        note.gameObject = noteObj;
        note.isActive = true;
        activeNotes[index] = note;
        
        if (showDebugInfo)
            Debug.Log($"[SimpleNoteSpawner] Spawned note for lane {note.lane} at time {note.time:F2}s");
    }

    private Vector3 CalculateNoteStartPosition(int lane)
    {
        if (hitTarget == null || lanePositions[lane] == null)
            return lanePositions[lane].position;
            
        Vector3 lanePos = lanePositions[lane].position;
        Vector3 targetPos = hitTarget.position;
        
        // Calculate offset from target based on travel time
        Vector3 startOffset = Vector3.zero;
        
        if (useXAxis)
        {
            // Notes move along X axis - spawn to the left if target is to the right
            startOffset.x = -travelTime * noteSpacing / 2f; // Adjust multiplier as needed
        }
        else
        {
            // Notes move along Y axis - spawn above if target is below
            startOffset.y = travelTime * noteSpacing / 2f;
        }
        
        return lanePos + startOffset;
    }

    private void UpdateNotePosition(MovingNote note, float currentTime)
    {
        if (note.gameObject == null || hitTarget == null) return;
        
        // Calculate how far along the note should be (0 to 1)
        // When currentTime == note.time, progress should be 1.0 (at hit target)
        float progress = (currentTime - note.spawnTime) / travelTime;
        progress = Mathf.Clamp01(progress);
        
        // Calculate positions
        Vector3 startPos = CalculateNoteStartPosition(note.lane);
        Vector3 laneTargetPos = lanePositions[note.lane].position;
        
        // Adjust target position to align with hit target
        if (useXAxis)
        {
            laneTargetPos.x = hitTarget.position.x;
        }
        else
        {
            laneTargetPos.y = hitTarget.position.y;
        }
        
        // Move note - it should be at laneTargetPos exactly when currentTime == note.time
        Vector3 currentPos = Vector3.Lerp(startPos, laneTargetPos, progress);
        note.gameObject.transform.position = currentPos;
        
        // Debug visualization for rhythm checking
        if (showTimingDebug && Mathf.Abs(currentTime - note.time) < 0.1f)
        {
            Debug.Log($"[Note Position] Time: {currentTime:F3}, Target: {note.time:F3}, Progress: {progress:F3}, Should be at hit target: {progress >= 0.95f}");
        }
    }

    private void TryHitNote()
    {
        float songTime = Time.time - songStartTime; // Current position in the song
        
        if (showTimingDebug)
            Debug.Log($"[Hit Attempt] Song Time: {songTime:F3}s, Mouse clicked!");
        
        MovingNote? closestNote = null;
        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        
        // Show all active notes and their timing
        if (showTimingDebug)
        {
            Debug.Log($"[Hit Check] Checking {activeNotes.Count} active notes:");
            for (int i = 0; i < activeNotes.Count; i++)
            {
                var note = activeNotes[i];
                if (note.isActive && note.gameObject != null)
                {
                    float timeDiff = Mathf.Abs(songTime - note.time);
                    float progress = (songTime - note.spawnTime) / travelTime;
                    Debug.Log($"  Note {i}: Target={note.time:F3}s, Diff={timeDiff:F3}s, Progress={progress:F2}, InWindow={timeDiff <= hitWindow}");
                }
            }
        }
        
        // Find the note closest to being at the hit target right now
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var note = activeNotes[i];
            if (!note.isActive || note.gameObject == null) continue;
            
            // Check if this note should be at the hit target right now
            float timeDiff = Mathf.Abs(songTime - note.time);
            
            if (timeDiff <= hitWindow && timeDiff < closestDistance)
            {
                closestDistance = timeDiff;
                closestNote = note;
                closestIndex = i;
            }
        }
        
        // Hit the closest note
        if (closestNote.HasValue && closestIndex >= 0)
        {
            // For rhythm accuracy, let's be more lenient about position
            // The key is timing, not position
            HitNote(closestNote.Value, closestIndex, closestDistance);
        }
        else
        {
            if (showDebugInfo)
            {
                // Show why no note was hit
                if (activeNotes.Count == 0)
                    Debug.Log($"[Miss] No active notes at song time {songTime:F3}s");
                else
                {
                    var nextNote = activeNotes.Where(n => n.isActive && n.time > songTime).OrderBy(n => n.time).FirstOrDefault();
                    if (nextNote.gameObject != null)
                        Debug.Log($"[Miss] Song time {songTime:F3}s - Next note at {nextNote.time:F3}s (in {nextNote.time - songTime:F2}s)");
                    else
                        Debug.Log($"[Miss] No notes in hit window ({hitWindow:F2}s) at song time {songTime:F3}s");
                }
            }
        }
    }

    private bool IsNoteAtHitPosition(MovingNote note, float songTime)
    {
        // Calculate how far the note has traveled (0 to 1)
        float progress = (songTime - note.spawnTime) / travelTime;
        
        // Note should be at hit position when progress is close to 1
        // Allow some tolerance for the hit window
        return progress >= 0.8f && progress <= 1.2f; // Adjust tolerance as needed
    }

    private void HitNote(MovingNote note, int index, float timeDiff)
    {
        float songTime = Time.time - songStartTime;
        
        // Calculate timing accuracy based on how close we are to the note's exact time
        float accuracy = 1f - (timeDiff / hitWindow);
        accuracy = Mathf.Clamp01(accuracy);
        
        // Determine hit quality
        string hitQuality = "Miss";
        int basePoints = 0;
        
        if (timeDiff <= hitWindow * 0.1f) // Within 10% of hit window
        {
            hitQuality = "Perfect!";
            basePoints = 100;
        }
        else if (timeDiff <= hitWindow * 0.3f) // Within 30% of hit window
        {
            hitQuality = "Great!";
            basePoints = 80;
        }
        else if (timeDiff <= hitWindow * 0.6f) // Within 60% of hit window
        {
            hitQuality = "Good";
            basePoints = 60;
        }
        else if (timeDiff <= hitWindow) // Within hit window
        {
            hitQuality = "Okay";
            basePoints = 40;
        }
        
        int finalPoints = Mathf.RoundToInt(basePoints * accuracy);
        score += finalPoints;
        
        // Destroy the note
        if (note.gameObject != null)
        {
            Destroy(note.gameObject);
        }
        
        // Remove from active list
        activeNotes.RemoveAt(index);
        
        // Detailed debug info
        if (showDebugInfo)
        {
            Debug.Log($"[SimpleNoteSpawner] {hitQuality} Hit!\n" +
                     $"Song Time: {songTime:F3}s | Note Time: {note.time:F3}s | Diff: {timeDiff:F3}s\n" +
                     $"Accuracy: {accuracy:F2} | Points: {finalPoints} | Total Score: {score}");
        }
    }

    private List<SimpleNote> LoadNotesFromChart()
    {
        var noteList = new List<SimpleNote>();
        
        try
        {
            string fullPath = System.IO.Path.Combine(songFolder, chartFileName);
            
            if (!System.IO.File.Exists(fullPath))
            {
                Debug.LogError($"[SimpleNoteSpawner] Chart file not found: {fullPath}");
                return noteList;
            }

            var chartReader = new ChartReader();
            var chart = chartReader.ReadChartFile(fullPath);
            var rawNotes = chart?.GetNotes(chartSection);

            if (rawNotes == null || rawNotes.Length == 0)
            {
                Debug.LogError($"[SimpleNoteSpawner] No notes found in section: {chartSection}");
                return noteList;
            }

            if (showDebugInfo)
                Debug.Log($"[SimpleNoteSpawner] Loaded {rawNotes.Length} raw notes from {chartSection}");

            // Convert to simple notes
            foreach (var note in rawNotes)
            {
                if (useSingleLane)
                {
                    // All notes go to single lane
                    noteList.Add(new SimpleNote(note.Seconds, singleLaneIndex));
                }
                else
                {
                    // Multi-lane mode
                    var buttons = note.ButtonIndexes;
                    if (buttons != null && buttons.Length > 0)
                    {
                        for (int i = 0; i < Mathf.Min(buttons.Length, 5); i++)
                        {
                            if (buttons[i])
                            {
                                noteList.Add(new SimpleNote(note.Seconds, i));
                            }
                        }
                    }
                    else
                    {
                        // Fallback to lane 0 if no button data
                        noteList.Add(new SimpleNote(note.Seconds, 0));
                    }
                }
            }

            // Sort by time
            noteList.Sort((a, b) => a.time.CompareTo(b.time));
            
            if (showDebugInfo)
                Debug.Log($"[SimpleNoteSpawner] Created {noteList.Count} simple notes");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SimpleNoteSpawner] Error loading chart: {e.Message}");
        }

        // Remove the old SpawnNotes method since we're using the new system
        
        return noteList;
    }

    private void ClearExistingNotes()
    {
        // Find and destroy all existing notes
        var existingNotes = GetComponentsInChildren<MovingNoteData>();
        for (int i = existingNotes.Length - 1; i >= 0; i--)
        {
            if (existingNotes[i] != null)
            {
                if (Application.isPlaying)
                    Destroy(existingNotes[i].gameObject);
                else
                    DestroyImmediate(existingNotes[i].gameObject);
            }
        }
        
        // Clear active notes list
        activeNotes.Clear();
        songRunning = false;
    }

    // Simple note data structure
    [System.Serializable]
    public struct SimpleNote
    {
        public float time;
        public int lane;

        public SimpleNote(float t, int l)
        {
            time = t;
            lane = l;
        }
    }

    // Moving note data structure for runtime
    [System.Serializable]
    public struct MovingNote
    {
        public float time;           // When note should be hit
        public int lane;            // Which lane
        public float spawnTime;     // When to spawn the note
        public bool hasSpawned;     // Has been spawned yet?
        public GameObject gameObject; // The spawned note object
        public bool isActive;       // Is currently active
    }

    // Component to attach to spawned note objects
    public class MovingNoteData : MonoBehaviour
    {
        public float targetTime;
        public int lane;
        public int noteIndex;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (lanePositions == null) return;

        // Draw lane positions and hit target
        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (lanePositions[i] == null) continue;
            
            Vector3 lanePos = lanePositions[i].position;
            
            // Lane position
            Gizmos.color = useSingleLane && i == singleLaneIndex ? Color.yellow : Color.cyan;
            Gizmos.DrawWireSphere(lanePos, 20f);
            
            // Note path from spawn to hit target
            if (hitTarget != null)
            {
                Vector3 startPos = CalculateNoteStartPosition(i);
                Vector3 targetPos = lanePos;
                
                // Adjust target to hit target position
                if (useXAxis)
                    targetPos.x = hitTarget.position.x;
                else
                    targetPos.y = hitTarget.position.y;
                
                // Draw note path
                Gizmos.color = Color.green;
                Gizmos.DrawLine(startPos, targetPos);
                
                // Draw spawn position
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(startPos, Vector3.one * 15f);
            }
        }
        
        // Draw hit target
        if (hitTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitTarget.position, 25f);
            
            // Draw hit window visualization
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Vector3 windowSize = useXAxis ? 
                new Vector3(hitWindow * noteSpacing, 50f, 1f) : 
                new Vector3(50f, hitWindow * noteSpacing, 1f);
            Gizmos.DrawCube(hitTarget.position, windowSize);
        }
        
        // Display score during play
        if (Application.isPlaying && songRunning)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 50, 
                $"Score: {score}\nActive Notes: {activeNotes.Count}\nTime: {Time.time - songStartTime:F1}s");
        }
    }
#endif
}