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
    [Tooltip("Five lane anchors (index 0..4). X is ignored at spawn; Y/Z/rot define the lane row.")]
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
    [Tooltip("Fallback prefab if a lane-specific prefab is not provided.")]
    public GameObject notePrefab;
    [Tooltip("Optional: lane-specific prefabs (0..4). If null, uses fallback 'notePrefab'.")]
    public GameObject[] lanePrefabs = new GameObject[5];
    [Tooltip("Initial pool size per lane (also used for the fallback pool).")]
    public int initialPoolPerLane = 24;

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

    // pools: 5 lane pools + 1 default pool
    private readonly Queue<GameObject>[] lanePools = new Queue<GameObject>[5];
    private readonly Queue<GameObject> defaultPool = new Queue<GameObject>();
    private Transform poolsRoot;

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
        poolsRoot = new GameObject("NotePools").transform;
        poolsRoot.SetParent(transform, false);

        // init per-lane pools
        for (int i = 0; i < 5; i++)
        {
            lanePools[i] = new Queue<GameObject>();
            var prefab = GetPrefabForLane(i);
            if (prefab != null)
                WarmPool(lanePools[i], prefab, initialPoolPerLane, $"Lane{i}_");
        }

        // also warm default pool if needed later
        if (notePrefab != null)
            WarmPool(defaultPool, notePrefab, initialPoolPerLane, "Default_");
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

        // Validate at least one prefab exists among lanePrefabs or fallback
        bool hasAnyPrefab = notePrefab != null;
        if (!hasAnyPrefab)
            for (int i = 0; i < 5; i++) if (lanePrefabs[i] != null) { hasAnyPrefab = true; break; }
        if (!hasAnyPrefab) { Debug.LogError("[ChartManager] No prefabs assigned (neither fallback nor any lane prefab)."); return; }

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
                int max = Mathf.Min(buttons?.Length ?? 0, 5);
                if (max == 0)
                {
                    int mappedFallback = (laneRemap != null && laneRemap.Length == 5) ? Mathf.Clamp(laneRemap[0], 0, 4) : 0;
                    allNotes.Add(new ChartNote { timeSec = n.Seconds, lane = mappedFallback });
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

        // Sort by time then lane so spawns are deterministic for chords
        allNotes.Sort((a, b) =>
        {
            int c = a.timeSec.CompareTo(b.timeSec);
            return c != 0 ? c : a.lane.CompareTo(b.lane);
        });

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

        GameObject go = Dequeue(cn.lane);
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

        // figure out which pool this instance belongs to by matching its prefab source (tagging is safer)
        // Simpler approach: use name prefix set at creation time to decide the pool.
        string n = mover.gameObject.name;
        int lane = -1;
        if (n.StartsWith("Lane0_")) lane = 0;
        else if (n.StartsWith("Lane1_")) lane = 1;
        else if (n.StartsWith("Lane2_")) lane = 2;
        else if (n.StartsWith("Lane3_")) lane = 3;
        else if (n.StartsWith("Lane4_")) lane = 4;

        if (lane >= 0) lanePools[lane].Enqueue(mover.gameObject);
        else defaultPool.Enqueue(mover.gameObject);
    }

    // ---------------- pools ----------------

    private void WarmPool(Queue<GameObject> pool, GameObject prefab, int count, string namePrefix)
    {
        for (int i = 0; i < Mathf.Max(1, count); i++)
        {
            var go = Instantiate(prefab, poolsRoot);
            go.name = namePrefix + i;
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    private GameObject Dequeue(int lane)
    {
        var prefab = GetPrefabForLane(lane);
        var pool = (prefab == null) ? defaultPool : lanePools[lane];

        // if empty, instantiate one more of the right prefab
        if (pool.Count == 0)
        {
            var go = Instantiate(prefab ?? notePrefab, poolsRoot);
            go.name = (prefab == null ? "Default_" : $"Lane{lane}_") + Guid.NewGuid().ToString("N").Substring(0, 8);
            go.SetActive(false);
            return go;
        }
        return pool.Dequeue();
    }

    private GameObject GetPrefabForLane(int lane)
    {
        if (lanePrefabs != null && lanePrefabs.Length == 5 && lanePrefabs[lane] != null)
            return lanePrefabs[lane];
        return notePrefab; // may be null; checked earlier in LoadChart
    }

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
