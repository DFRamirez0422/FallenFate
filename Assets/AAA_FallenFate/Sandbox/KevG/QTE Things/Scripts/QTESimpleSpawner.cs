using UnityEngine;

public class QTESimpleSpawner : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform canvasRoot;     // optional; used to calculate spawn Y
    public QTENote notePrefab;           // note prefab (UI Image + QTENote)

    [Header("Holders")]
    public QTEHolder[] holders;          // lanes (each has a key)

    [Header("Spawn Settings")]
    public float spawnTopOffset = 100f;  // how far above top edge
    public float defaultFallSpeed = 900f; // default speed

    void Update()
    {
        // spawn a random note when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomNote();
        }
    }

    void SpawnRandomNote()
    {
        if (holders == null || holders.Length == 0 || notePrefab == null) return;

        // pick a random lane
        QTEHolder lane = holders[Random.Range(0, holders.Length)];
        if (lane == null) return;

        SpawnNote(lane);
    }

    public QTENote SpawnNote(QTEHolder lane) // spawns aligned to lane X
    {
        if (notePrefab == null || lane == null) return null;

        // parent under the same group as holders so coordinates match
        var parent = lane.transform.parent as RectTransform;
        if (parent == null) parent = canvasRoot; // fallback
        if (parent == null) parent = transform as RectTransform;

        var note = Instantiate(notePrefab, parent);

        // X = holder X, Y = above top
        var noteRT   = note.GetComponent<RectTransform>();
        var holderRT = lane.Rect;

        float topY = GetTopY(parent);
        noteRT.anchoredPosition = new Vector2(holderRT.anchoredPosition.x, topY + spawnTopOffset);

        // set fields
        note.SetTargetHolder(lane);
        note.fallSpeed = defaultFallSpeed;

        note.gameObject.SetActive(true);
        return note;
    }

    float GetTopY(RectTransform parent) // approximate top Y in anchored space
    {
        var root = canvasRoot != null ? canvasRoot : parent;
        return root != null ? (root.rect.height * 0.5f) : 0f;
    }
}
