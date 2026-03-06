using System.Collections.Generic;
using UnityEngine;

public class PathCycleManager : MonoBehaviour
{
    [Header("Grouped Path Sets")]
    public List<PathGroup> PathSets;

    [Header("Timer Settings")]
    public float PathEnabledTimer = 30f;

    [Header("Fade Settings")]
    public float FadeSpeed = 2f;

    [Range(0f,1f)]
    public float FadeOutAlpha = 0f;

    [Header("Runtime State")]
    public int counterValue = 0;

    float timer;
    bool timerPaused;

    void Start()
    {
        ApplyCurrentState();
    }

    void Update()
    {
        if (timerPaused) return;
        if (PathSets.Count == 0 || PathSets[0].Paths.Count == 0) return;

        timer += Time.deltaTime;

        if (timer > PathEnabledTimer)
        {
            timer = 0f;

            counterValue++;

            if (counterValue >= PathSets[0].Paths.Count)
                counterValue = 0;

            ApplyCurrentState();
        }
    }

    void ApplyCurrentState()
    {
        foreach (var set in PathSets)
        {
            ProcessPathSet(set.Paths);
            SetParentEdgeColliders(set.Paths);
        }
    }

    void ProcessPathSet(List<GameObject> pathSet)
    {
        for (int i = 0; i < pathSet.Count; i++)
        {
            ObjectFader fader = pathSet[i].GetComponent<ObjectFader>();
            if (fader == null) continue;

            fader.fadeSpeed = FadeSpeed;

            if (i == counterValue)
            {
                fader.DoFade = false;
            }
            else
            {
                fader.fadeAmount = FadeOutAlpha;
                fader.DoFade = true;
            }
        }
    }

    void SetParentEdgeColliders(List<GameObject> pathSet)
    {
        EdgeCollider2D[] edges = pathSet[0].transform.parent.GetComponents<EdgeCollider2D>();

        for (int i = 0; i < edges.Length; i++)
        {
            edges[i].enabled = (i == counterValue);
        }
    }

    public void PauseTimer() => timerPaused = true;

    public void ResumeTimer() => timerPaused = false;
}