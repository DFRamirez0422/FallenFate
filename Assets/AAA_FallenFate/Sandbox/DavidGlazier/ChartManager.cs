using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ChartLoader.NET.Framework;
using ChartLoader.NET.Utils;

public class ChartManager : MonoBehaviour
{
    public enum SpawnMode { MultiLane, SingleLane }

    [Header("Chart Setup")]
    public string songFolder = "Assets/ChartLoader/ChartLoader/Songs/DOOM";
    public string chartFileName = "notes.chart";
    [Tooltip("e.g. ExpertSingle / HardSingle / EasySingle etc.")]
    public string chartSection = "ExpertSingle";

    [Header("Timing")]
    [Tooltip("How long a note takes to travel from spawn to target.")]
    public float travelTimeSec = 1.50f;
    [Tooltip("Positive spawns earlier to compensate latency.")]
    public float latencyOffsetSec = 0f;

    [Header("Lane & Target")]
    [Tooltip("Five lane anchors (index 0..4). X is ignored at spawn; Y/Z/rot define lane row.")]
    public Transform[] laneAnchors = new Transform[5];
    [Tooltip("Right-side target; notes destroy when they reach this x.")]
    public Transform target;
    [Tooltip("X coordinate where notes appear (must be < target.x for left→right).")]
    public float spawnX = -8f;
    public float hitRadius = 0.1f;

    [Header("Mode")]
    public SpawnMode spawnMode = SpawnMode.MultiLane;
    [Range(0,4)] public int singleLaneIndex = 0;

    [Header("Lane Mapping (MultiLane only)")]
    [Tooltip("Optional remap incoming fret 0..4 to anchor index 0..4.")]
    public int[] laneRemap = new int[5] { 0, 1, 2, 3, 4 };

    [Header("Prefabs & Pooling")]
    public GameObject notePrefab;
    public int initialPool = 64;

    [Header("Debug")]
    public bool autoStart = true;
    public bool logSpawns = false;
    [Tooltip("Log first N parsed timestamps+lanes after load.")]
    public int debugPreviewCount = 24;

    // ---------------- internals ----------------
    private ChartReader chartReader;
    private List<ChartNote> allNotes = new List<ChartNote>(); // flattened list: one entry per visual lane-hit
    private int spawnIndex;
    private double songStartTime;
    private bool running;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private Transform poolRoot;

    [Serializable]
    private struct ChartNote { public double timeSec; public int lane; }

    private class NoteMover : MonoBehaviour
    {
        public Transform target;
        public float speed;
        public float hitRadius;
        public Action<NoteMover> OnDespawn;
        private bool armed;

        public void Arm(Transform t, float s, float r)
        { target = t; speed = s; hitRadius = r; armed = true; }

        void Update()
        {
            if (!armed || target == null) return;
            var p = transform.position;
            float dir = Mathf.Sign(target.position.x - p.x); // expect +1 (left→right)
            p.x += dir * speed * Time.deltaTime;
            transform.position = p;

            float dx = Mathf.Abs(target.position.x - p.x);
            if (dx <= hitRadius || (dir > 0 && p.x >= target.position.x) || (dir < 0 && p.x <= target.position.x))
            {
                armed = false;
                OnDespawn?.Invoke(this);
            }
        }
    }

    void Awake()
    {
        poolRoot = new GameObject("NotePool").transform;
        poolRoot.SetParent(transform, false);
        if (notePrefab != null)
            for (int i = 0; i < Mathf.Max(1, initialPool); i++) Enqueue(NewNote());
    }

    void Start()
    {
        if (autoStart)
        {
            LoadChart();
            StartRun();
        }
    }

    void Update()
    {
        if (!running || allNotes.Count == 0) return;

        double t = Time.timeAsDouble - songStartTime;

        // spawn when it's time so the piece arrives exactly on its beat
        while (spawnIndex < allNotes.Count)
        {
            double spawnTime = allNotes[spawnIndex].timeSec - travelTimeSec - latencyOffsetSec;
            if (t >= spawnTime) { Spawn(allNotes[spawnIndex]); spawnIndex++; }
            else break;
        }
    }

    // ---------------- loading ----------------

    public void LoadChart()
    {
        if (target == null) { Debug.LogError("[ChartManager] Assign a target Transform."); return; }
        if (laneAnchors == null || laneAnchors.Length != 5) { Debug.LogError("[ChartManager] Provide exactly 5 lane anchors."); return; }
        if (notePrefab == null) { Debug.LogError("[ChartManager] Assign a note prefab."); return; }

        string fullPath = Path.Combine(songFolder, chartFileName);
        if (!File.Exists(fullPath)) { Debug.LogError("[ChartManager] Chart not found: " + fullPath); return; }

        chartReader = new ChartReader();
        Chart chart = chartReader.ReadChartFile(fullPath);

        // pull notes for the selected difficulty/section
        Note[] raw = chart?.GetNotes(chartSection);
        if (raw == null || raw.Length == 0) { Debug.LogError($"[ChartManager] No notes in section: {chartSection}"); return; }

        allNotes.Clear();

        if (spawnMode == SpawnMode.SingleLane)
        {
            int forced = Mathf.Clamp(singleLaneIndex, 0, 4);
            foreach (var n in raw)
            {
                allNotes.Add(new ChartNote { timeSec = n.Seconds, lane = forced });
            }
        }
        else // MultiLane — read ButtonIndexes[0..4] and make one entry per true fret
        {
            foreach (var n in raw)
            {
                bool[] buttons = n.ButtonIndexes; // GH frets Green..Orange
                // Safety: some charts/tools may have longer arrays; we only care about first 5
                int max = Mathf.Min(buttons?.Length ?? 0, 5);
                if (max == 0)
                {
                    // Fallback: if ButtonIndexes is missing/empty (very rare), just assume fret 0
                    int mapped = (laneRemap != null && laneRemap.Length == 5) ? Mathf.Clamp(laneRemap[0], 0, 4) : 0;
                    allNotes.Add(new ChartNote { timeSec = n.Seconds, lane = mapped });
                    continue;
                }

                for (int fret = 0; fret < max; fret++)
                {
                    if (!buttons[fret]) continue;
                    int mapped = fret;
                    if (laneRemap != null && laneRemap.Length == 5)
                        mapped = Mathf.Clamp(laneRemap[fret], 0, 4);

                    allNotes.Add(new ChartNote { timeSec = n.Seconds, lane = mapped });
                }
            }
        }

        // Sort by time so spawns are chronological even after chord expansion
        allNotes.Sort((a, b) =>
        {
            int c = a.timeSec.CompareTo(b.timeSec);
            return c != 0 ? c : a.lane.CompareTo(b.lane);
        });

        // Preview a few for sanity
        if (debugPreviewCount > 0)
        {
            int count = Mathf.Min(debugPreviewCount, allNotes.Count);
            for (int i = 0; i < count; i++)
                Debug.Log($"[ChartManager] t={allNotes[i].timeSec:F3}s lane={allNotes[i].lane}");
        }

        if (logSpawns) Debug.Log($"[ChartManager] Loaded {allNotes.Count} lane-hits from {chartSection}.");
    }

    // ---------------- run control ----------------

    public void StartRun()
    {
        spawnIndex = 0;
        songStartTime = Time.timeAsDouble;
        running = true;
    }

    public void StopRun(bool clearActive = true)
    {
        running = false;
        if (clearActive)
        {
            foreach (var mover in FindObjectsOfType<NoteMover>())
                mover?.OnDespawn?.Invoke(mover);
        }
    }

    // ---------------- spawning ----------------

    private void Spawn(ChartNote cn)
    {
        if (cn.lane < 0 || cn.lane >= laneAnchors.Length) return;

        Vector3 lanePos = laneAnchors[cn.lane].position;
        Vector3 spawnPos = new Vector3(spawnX, lanePos.y, lanePos.z);

        float distance = Mathf.Abs(target.position.x - spawnPos.x);
        float speed = distance / Mathf.Max(0.0001f, travelTimeSec);

        GameObject go = Dequeue();
        go.transform.SetPositionAndRotation(spawnPos, laneAnchors[cn.lane].rotation);
        go.SetActive(true);

        var mover = go.GetComponent<NoteMover>();
        if (mover == null) mover = go.AddComponent<NoteMover>();
        mover.OnDespawn = Despawn;
        mover.Arm(target, speed, hitRadius);

        if (logSpawns) Debug.Log($"[ChartManager] Spawn t={cn.timeSec:F3}s lane={cn.lane} speed={speed:F3}");
    }

    private void Despawn(NoteMover mover)
    {
        if (mover == null) return;
        mover.gameObject.SetActive(false);
        Enqueue(mover.gameObject);
    }

    // ---------------- pool ----------------

    private GameObject NewNote()
    {
        var go = Instantiate(notePrefab, poolRoot);
        go.name = "Note";
        go.SetActive(false);
        return go;
    }
    private void Enqueue(GameObject go) => pool.Enqueue(go);
    private GameObject Dequeue() => pool.Count > 0 ? pool.Dequeue() : NewNote();

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target == null || laneAnchors == null) return;
        Gizmos.color = new Color(0.2f, 0.9f, 0.9f, 0.6f);
        foreach (var lane in laneAnchors)
        {
            if (lane == null) continue;
            Vector3 a = new Vector3(spawnX, lane.position.y, lane.position.z);
            Vector3 b = new Vector3(target.position.x, lane.position.y, lane.position.z);
            Gizmos.DrawLine(a, b);
        }
        Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.8f);
        Gizmos.DrawWireSphere(target.position, hitRadius);
    }
#endif
}
