using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QTENote : MonoBehaviour
{
    [Header("Target Lane")]
    [SerializeField, HideInInspector]
    private QTEHolder targetHolder; // set by spawner only
    public QTEHolder TargetHolder => targetHolder; // read-only

    [Header("Movement")]
    public float fallSpeed = 500f; // note falling speed
    public bool useUnscaledTime = true; // ignore time scale (works during pause)

    [Header("Colors")]
    public Image body; // note image
    public Color normalColor = Color.white; // default color
    public Color hitColor = Color.white; // color on hit
    public Color missColor = Color.red; // color on miss or wrong key

    [Header("Pop Effect")]
    public float popScale = 1.25f; // scale size when hit
    public float popDuration = 0.12f; // scale up speed
    public float shrinkDuration = 0.10f; // scale down speed

    RectTransform rt; // cached rect
    bool resolved = false; // prevents double hit/miss

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (!body) body = GetComponent<Image>();
        if (body) body.color = normalColor;
    }

    void Update()
    {
        if (resolved) return;

        // move note down the screen
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        rt.anchoredPosition += Vector2.down * fallSpeed * dt;

        // if note passes below holder → miss
        if (targetHolder)
        {
            float holderY = targetHolder.Rect.anchoredPosition.y;
            if (rt.anchoredPosition.y < holderY - targetHolder.hitWindowHalfHeight)
            {
                Miss();
            }
        }

        // check input only when inside hit window
        if (targetHolder && Input.anyKeyDown)
        {
            float dy = Mathf.Abs(rt.anchoredPosition.y - targetHolder.Rect.anchoredPosition.y);
            bool inWindow = dy <= targetHolder.hitWindowHalfHeight;

            if (inWindow)
            {
                // correct key pressed
                if (Input.GetKeyDown(targetHolder.assignedKey))
                {
                    Hit();
                }
                else
                {
                    // wrong key inside window
                    if (AnyNonModifierKeyDown() && !Input.GetKeyDown(targetHolder.assignedKey))
                        Miss();
                }
            }
        }
    }

    bool AnyNonModifierKeyDown() // ignore Shift, Ctrl, Alt, etc.
    {
        return Input.inputString.Length > 0;
    }

    public void SetTargetHolder(QTEHolder holder) // called by spawner
    {
        targetHolder = holder;
    }

    void Hit()
    {
        if (resolved) return;
        resolved = true;

        if (body) body.color = hitColor;
        Debug.Log("Hit!");
        StartCoroutine(PopAndDestroy()); // scale up then shrink
    }

    void Miss()
    {
        if (resolved) return;
        resolved = true;

        if (body) body.color = missColor;
        Debug.Log("Missed!");
        StartCoroutine(ShrinkAndDestroy()); // shrink and destroy
    }

    IEnumerator PopAndDestroy()
    {
        Vector3 start = rt.localScale;
        Vector3 pop = start * popScale;

        // grow
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / popDuration;
            rt.localScale = Vector3.Lerp(start, pop, k);
            yield return null;
        }

        // shrink
        t = 0f;
        while (t < shrinkDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / shrinkDuration;
            rt.localScale = Vector3.Lerp(pop, Vector3.zero, k);
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy()
    {
        Vector3 start = rt.localScale;
        float t = 0f;
        float dur = 0.12f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            rt.localScale = Vector3.Lerp(start, Vector3.zero, k);
            yield return null;
        }

        Destroy(gameObject);
    }
}
