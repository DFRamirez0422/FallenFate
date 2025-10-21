// This code handles the falling logic
// How fast the note will fall
// Detects if it's in the hitbox when lane key is pressed
// Changes color on perfect/partial/miss hits
// Handles destroy animations

using System.Collections; 
using UnityEngine; 
using UnityEngine.UI; 

public class QTENote : MonoBehaviour
{
    [SerializeField, HideInInspector] private QTELane targetLane; // set by manager when spawned
    public QTELane TargetLane => targetLane; // allows reading which lane it belongs to
    [HideInInspector] public QTEManager manager; // reference to main QTE manager

    [Header("Settings")]
    public bool useUnscaledTime = true; // ignore game pauses
    private float fallSpeed; // assigned by manager when spawned

    [Header("Colors")]
    public Image body; // note’s image component
    public Color normalColor = Color.white; // default color
    public Color perfectColor = Color.green; // perfect hit color
    public Color partialColor = Color.yellow; // near miss color
    public Color missColor = Color.red; // missed color

    [Header("Pop Effect")]
    public float popScale = 1.25f; // how big it pops
    public float popTime = 0.12f; // time to grow
    public float shrinkTime = 0.10f; // time to shrink

    RectTransform rt; // note position on canvas
    bool done; // if already judged

    void Awake()
    {
        rt = GetComponent<RectTransform>(); // get rect of note
        if (!body) body = GetComponent<Image>(); // find image if not set
        if (body) body.color = normalColor; // set starting color
    }

    void Update()
    {
        if (done || manager == null || targetLane == null) return; // stop if note done or missing info

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime; // choose correct time step
        rt.anchoredPosition += Vector2.down * fallSpeed * dt; // move note down every frame

        var hitRT = manager.hitBox.rectTransform; // get hitbox rect
        float hitBottomWorld = GetWorldBottomY(hitRT); // find bottom of hitbox
        float noteCenterWorld = GetWorldCenterY(rt); // find center of note
        if (noteCenterWorld < hitBottomWorld - manager.partialRange) // if too low
        {
            Miss(); // mark as missed
        }
    }

    public RectTransform Rect => rt; // get note’s rect

    public void SetTargetLane(QTELane lane)
    {
        targetLane = lane; // set which lane it belongs to
    }

    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed; // assign fall speed
    }

    public void TryHitByLane()
    {
        if (done || manager == null) return; // stop if invalid or done

        RectTransform hitRT = manager.hitBox.rectTransform; // get hitbox
        float hitTopWorld = GetWorldTopY(hitRT); // top of hitbox
        float hitBottomWorld = GetWorldBottomY(hitRT); // bottom of hitbox
        float noteCenterWorld = GetWorldCenterY(rt); // center of note

        // if note center is inside hitbox: perfect
        if (noteCenterWorld <= hitTopWorld && noteCenterWorld >= hitBottomWorld)
        {
            Hit(true);
            return;
        }

        // if slightly outside: partial
        bool nearAbove = noteCenterWorld > hitTopWorld && noteCenterWorld <= hitTopWorld + manager.partialRange;
        bool nearBelow = noteCenterWorld < hitBottomWorld && noteCenterWorld >= hitBottomWorld - manager.partialRange;

        if (nearAbove || nearBelow)
        {
            Hit(false);
        }
        else
        {
            Miss(); // too far away
        }
    }

    float GetWorldTopY(RectTransform r)
    {
        Vector3[] c = new Vector3[4]; // store corners
        r.GetWorldCorners(c); // fill array
        return c[1].y; // top Y
    }

    float GetWorldBottomY(RectTransform r)
    {
        Vector3[] c = new Vector3[4];
        r.GetWorldCorners(c);
        return c[0].y; // bottom Y
    }

    float GetWorldCenterY(RectTransform r)
    {
        Vector3[] c = new Vector3[4];
        r.GetWorldCorners(c);
        return 0.5f * (c[1].y + c[0].y); // midpoint between top and bottom
    }

    void Hit(bool perfect)
    {
        if (done) return; // skip if already judged
        done = true; // mark as done

        if (body) body.color = perfect ? perfectColor : partialColor; // set color
        if (!perfect) manager?.RegisterPartial(); // add to partials if needed

        targetLane.activeNotes.Remove(this); // remove from lane
        Debug.Log(perfect ? "Perfect!" : "Good!"); // print feedback
        StartCoroutine(PopAndDestroy()); // play animation
    }

    void Miss()
    {
        if (done) return; // skip if already judged
        done = true; // mark as done

        if (body) body.color = missColor; // change color to red
        manager?.RegisterMiss(); // add to miss counter

        targetLane.activeNotes.Remove(this); // remove from lane
        Debug.Log("Missed!"); // print feedback
        StartCoroutine(ShrinkAndDestroy()); // shrink animation
    }

    IEnumerator PopAndDestroy()
    {
        Vector3 start = rt.localScale; // start size
        Vector3 pop = start * popScale; // bigger size
        float t = 0f; // timer start

        while (t < popTime) // grow phase
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(start, pop, t / popTime); // scale up
            yield return null; // wait one frame
        }

        t = 0f; // reset timer
        while (t < shrinkTime) // shrink phase
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(pop, Vector3.zero, t / shrinkTime); // scale down
            yield return null;
        }

        Destroy(gameObject); // remove note
    }

    IEnumerator ShrinkAndDestroy()
    {
        Vector3 start = rt.localScale; // start size
        float t = 0f; // timer start

        while (t < 0.12f) // shrink quickly
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(start, Vector3.zero, t / 0.12f); // scale down
            yield return null;
        }

        Destroy(gameObject); // remove note
    }
}
