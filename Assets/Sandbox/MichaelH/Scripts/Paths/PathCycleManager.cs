using System.Collections.Generic;
using UnityEngine;

public class PathCycleManager : MonoBehaviour
{
    [Header("Grouped Path Sets")]
    public List<PathGroup> PathSets;

    [Header("Timer")]
    public float PathEnabledTimer = 30f;

    [Header("Fade Settings")]
    public float FadeSpeed = 2f;

    [Range(0f, 1f)]
    public float FadeOutAlpha = 0f;

    [Header("Runtime")]
    public int currentFadingSet = 0; // 0 = Set 1 fades, 1 = Set 2 fades

    // Timer variables
    float timer;
    bool timerPaused;

    void Start()
    {
        currentFadingSet = 0;
        UpdatePaths();
    }

    /// <summary>
    ///  Cycles between the two path sets, fading one out while keeping the other visible. The timer can be paused by PathPauseTriggers on the paths.
    /// </summary>
    void Update()
    {
        if (timerPaused || PathSets.Count != 2)
            return;

        timer += Time.deltaTime;

        if (timer >= PathEnabledTimer)
        {
            timer = 0f;

            currentFadingSet = (currentFadingSet + 1) % 2;
            UpdatePaths();
        }
    }

    /// <summary>
    /// Updates the fade settings on all paths based on which set is currently fading. The fading set will fade out to the specified alpha, while the other set will stay fully visible.
    /// </summary>
    void UpdatePaths()
    {
        for (int s = 0; s < PathSets.Count; s++)
        {
            var set = PathSets[s];

            for (int i = 0; i < set.Paths.Count; i++)
            {
                ObjectFader fader = set.Paths[i].GetComponent<ObjectFader>();
                if (!fader) continue;

                fader.fadeSpeed = FadeSpeed;

                if (s == currentFadingSet)
                {
                    // This whole set fades out
                    fader.fadeAmount = FadeOutAlpha;
                    fader.DoFade = true;
                }
                else
                {
                    // The other whole set stays visible
                    fader.DoFade = false;
                }
            }
        }
    }

    /// <summary>
    /// Called by PathPauseTriggers on the paths to pause/resume the timer when the player is on a path tile. When paused, the current fading set will remain unchanged until the timer is resumed.
    /// </summary>
    public void PauseTimer() => timerPaused = true;
    public void ResumeTimer() => timerPaused = false;
}