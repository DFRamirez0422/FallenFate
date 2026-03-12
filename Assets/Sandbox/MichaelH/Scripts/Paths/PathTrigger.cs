using UnityEngine;

public class PathPauseTrigger : MonoBehaviour
{
    public PathCycleManager manager;

    [Tooltip("Which set this tile belongs to")]
    public int setIndex;

    /// <summary>
    /// If player is standing on a tile that belongs to the currently visible set, pause the timer to prevent paths from changing while player is on them. Resume the timer when player steps off.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Pause if player is standing on the visible set
        if (setIndex != manager.currentFadingSet)
        {
            manager.PauseTimer();
        }
    }

    /// <summary>
    /// If player steps off the tile, resume the timer so paths can continue to fade in sequence. If player steps onto another tile, the timer will be paused again in OnTriggerStay2D.
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        manager.ResumeTimer();
    }
}