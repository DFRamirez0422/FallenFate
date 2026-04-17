using System.Collections;
using UnityEngine;

public class CollectibleSparkler : MonoBehaviour
{
    [Header("Sparkle References")]
    [Tooltip("Assign the sparkle child objects here.")]
    [SerializeField] private GameObject[] sparkleObjects;

    [Header("Timing")]
    [Tooltip("How long each sparkle stays visible.")]
    [SerializeField] private float sparkleVisibleDuration = 0.35f;

    [Tooltip("How long to wait before starting the next sparkle in the sequence. Lower than visible duration creates overlap.")]
    [SerializeField] private float timeBetweenSparkles = 0.22f;

    [Tooltip("How long to wait after a full sparkle cycle before playing again.")]
    [SerializeField] private float delayBetweenCycles = 4f;

    [Header("Pattern")]
    [Tooltip("If true, each cycle shuffles the sparkle order so it does not always play 1-2-3.")]
    [SerializeField] private bool randomizeOrderEachCycle = true;

    [Tooltip("If true, the script tries to avoid starting the new cycle with the same sparkle that ended the last one.")]
    [SerializeField] private bool avoidSameStartAsLastEnd = true;

    [Header("Behavior")]
    [Tooltip("If true, start sparkling automatically when enabled.")]
    [SerializeField] private bool playOnEnable = true;

    private Coroutine sparkleRoutine;
    private int lastSparkleFromPreviousCycle = -1;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartSparkleLoop();
        }
    }

    private void OnDisable()
    {
        if (sparkleRoutine != null)
        {
            StopCoroutine(sparkleRoutine);
            sparkleRoutine = null;
        }

        SetAllSparkles(false);
    }

    private void Reset()
    {
        int childCount = transform.childCount;
        sparkleObjects = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            sparkleObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    public void StartSparkleLoop()
    {
        if (sparkleRoutine != null)
        {
            StopCoroutine(sparkleRoutine);
        }

        sparkleRoutine = StartCoroutine(SparkleLoop());
    }

    public void StopSparkleLoop()
    {
        if (sparkleRoutine != null)
        {
            StopCoroutine(sparkleRoutine);
            sparkleRoutine = null;
        }

        SetAllSparkles(false);
    }

    private IEnumerator SparkleLoop()
    {
        while (true)
        {
            if (sparkleObjects == null || sparkleObjects.Length == 0)
            {
                yield return null;
                continue;
            }

            int[] order = BuildSparkleOrder();

            for (int i = 0; i < order.Length; i++)
            {
                int sparkleIndex = order[i];

                if (sparkleIndex >= 0 && sparkleIndex < sparkleObjects.Length && sparkleObjects[sparkleIndex] != null)
                {
                    StartCoroutine(ShowSparkleForDuration(sparkleObjects[sparkleIndex], sparkleVisibleDuration));
                }

                yield return new WaitForSeconds(timeBetweenSparkles);
            }

            lastSparkleFromPreviousCycle = order[order.Length - 1];

            float remainingOverlapTime = Mathf.Max(0f, sparkleVisibleDuration - timeBetweenSparkles);
            if (remainingOverlapTime > 0f)
            {
                yield return new WaitForSeconds(remainingOverlapTime);
            }

            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }

    private IEnumerator ShowSparkleForDuration(GameObject sparkleObject, float duration)
    {
        if (sparkleObject == null)
            yield break;

        sparkleObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        sparkleObject.SetActive(false);
    }

    private int[] BuildSparkleOrder()
    {
        int count = sparkleObjects.Length;
        int[] order = new int[count];

        for (int i = 0; i < count; i++)
        {
            order[i] = i;
        }

        if (randomizeOrderEachCycle)
        {
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(i, count);
                int temp = order[i];
                order[i] = order[randomIndex];
                order[randomIndex] = temp;
            }

            if (avoidSameStartAsLastEnd && count > 1 && order[0] == lastSparkleFromPreviousCycle)
            {
                int swapIndex = Random.Range(1, count);
                int temp = order[0];
                order[0] = order[swapIndex];
                order[swapIndex] = temp;
            }
        }

        return order;
    }

    private void SetAllSparkles(bool isActive)
    {
        if (sparkleObjects == null)
            return;

        for (int i = 0; i < sparkleObjects.Length; i++)
        {
            if (sparkleObjects[i] != null)
            {
                sparkleObjects[i].SetActive(isActive);
            }
        }
    }
}