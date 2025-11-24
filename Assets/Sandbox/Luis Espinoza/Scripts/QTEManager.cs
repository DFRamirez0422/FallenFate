// This code controls the QTE spawn sequence
// Spawns notes randomly into the lanes
// Keeps track of active notes
// Handles win/loss tracking (perfect, partial, miss)
// Can be called with QTEManager.Instance.StartQTE();
// Press SPACE to spawn wave

using System.Collections; 
using UnityEngine; 
using UnityEngine.UI; 

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance; // global access to this script

    [Header("QTE")]
    public GameObject QTERoot; // parent for all QTE UI

    [Header("References")]
    public QTENote notePrefab; // prefab for note
    public Image hitBox; // invisible hit area
    public QTELane[] lanes; // list of all lanes

    [Header("Spawn Settings")]
    public float spawnTopOffset = 500f; // height to spawn notes from
    public float fallSpeed = 300f; // how fast they fall
    public float spawnInterval = 1f; // time between spawns
    public int[] spawnSet = { 4, 7, 11 }; // number of notes per set

    [Header("Hitbox Settings")]
    public float partialRange = 50f; // near-miss distance

    [Header("Fail Settings")]
    public int loseAfter = 3; // how many fails to lose
    int partialCount; // partial hit counter
    int missCount; // miss counter

    // NEW: boss QTE fail conditions (tweakable in inspector)
    public int partialFailThreshold = 3; // fail if partialCount > this
    public int missFailThreshold = 1;    // fail if missCount >= this

    // NEW: optional override for how many notes to spawn this run
    int forcedCount = 0;
    bool useForcedCount = false;

    bool spawning; // if notes are spawning
    bool qteActive; // if QTE is running
    public bool IsActive => qteActive; // read-only for other scripts
    public int PartialCount => partialCount;
    public int MissCount => missCount;


    void Awake()
    {
        Instance = this; // set global reference
        if (QTERoot) QTERoot.SetActive(false); // hide QTE on start
    }

    void Update()
    {
        /* Commenting this out to test calling this only from the BossAI script
        if (Input.GetKeyDown(KeyCode.Space)) // space to test
            StartQTE(); // start QTE manually
        */
    }

    public void StartQTE()
    {
        if (qteActive) return; // stop if already running
        StartCoroutine(RunQTECycle()); // start sequence
    }

    // NEW: start QTE but force a specific spawnSet entry (0 = first, 1 = second, 2 = third...)
    public void StartQTEWithPresetIndex(int presetIndex)
    {

        if (qteActive) return;

        if (spawnSet != null && spawnSet.Length > 0 &&
            presetIndex >= 0 && presetIndex < spawnSet.Length)
        {
            forcedCount = spawnSet[presetIndex];
            useForcedCount = true;
        }
        else
        {
            useForcedCount = false; // fallback to normal random behavior
        }

        StartCoroutine(RunQTECycle());
    }

    IEnumerator RunQTECycle()
    {
        ActivateQTE(); // enable QTE UI
        ResetCounts(); // reset scores

        yield return StartCoroutine(SpawnSetOnce()); // spawn notes
        yield return StartCoroutine(WaitUntilAllNotesCleared()); // wait till all gone
        yield return new WaitForSeconds(1f); // small delay
        DeactivateQTE(); // hide QTE again
    }

    void ActivateQTE()
    {
        qteActive = true; // mark active
        if (QTERoot) QTERoot.SetActive(true); // show UI
    }

    void DeactivateQTE()
    {
        qteActive = false; // mark inactive
        if (QTERoot) QTERoot.SetActive(false); // hide UI
    }

    IEnumerator SpawnSetOnce()
    {
        spawning = true; // mark spawning

        int count;

        // NEW: use a forced count once if set, otherwise use the random spawnSet logic
        if (useForcedCount)
        {
            count = forcedCount;
            useForcedCount = false; // consume the override
        }
        else
        {
            count = spawnSet.Length > 0
                ? spawnSet[Random.Range(0, spawnSet.Length)] // pick random amount
                : 0; // none if empty
        }

        for (int i = 0; i < count; i++) // repeat notes
        {
            SpawnRandomNote(); // create note
            if (i < count - 1)
                yield return new WaitForSeconds(spawnInterval); // wait between notes
        }

        spawning = false; // done spawning
    }

    IEnumerator WaitUntilAllNotesCleared()
    {
        while (TotalActiveNotes() > 0) // while notes exist
            yield return null; // wait frame
    }

    void SpawnRandomNote()
    {
        if (lanes.Length == 0 || !notePrefab) return; // no data stop
        var lane = lanes[Random.Range(0, lanes.Length)]; // pick random lane
        SpawnNote(lane); // spawn note in it
    }

    public QTENote SpawnNote(QTELane lane)
    {
        if (!lane || !notePrefab) return null; // stop if invalid

        var parent = lane.transform.parent as RectTransform; // get canvas
        if (!parent) return null; // stop if missing

        var note = Instantiate(notePrefab, parent); // make note
        var noteRT = note.GetComponent<RectTransform>(); // get rect
        var laneRT = lane.Rect; // get lane rect

        float topY = parent.rect.height * 0.5f; // find top
        noteRT.anchoredPosition = new Vector2(laneRT.anchoredPosition.x, topY + spawnTopOffset); // place note

        note.SetTargetLane(lane); // link lane
        note.SetFallSpeed(fallSpeed); // set speed
        note.manager = this; // link manager

        lane.activeNotes.Add(note); // track it
        return note; // return note
    }

    public int TotalActiveNotes()
    {
        int n = 0; // counter
        for (int i = 0; i < lanes.Length; i++) // loop lanes
        {
            if (lanes[i] != null && lanes[i].activeNotes != null)
                n += lanes[i].activeNotes.Count; // add lane notes
        }
        return n; // return count
    }

    public void RegisterPartial()
    {
        partialCount++; // track partial hits
    }

    public void RegisterMiss()
    {
        missCount++; // track misses

        if (missCount >= loseAfter) // if too many
        {
            Debug.Log("QTE failed!"); // log failure
            // handle failure here later
        }
    }

    void CheckLose()
    {
        if (partialCount + missCount >= loseAfter) // if too many fails
            Debug.Log("You Lose!"); // show lose
    }

    public void ResetCounts()
    {
        partialCount = 0; // reset partials
        missCount = 0; // reset misses
    }
}