using UnityEngine;

public class PathPauseTrigger : MonoBehaviour
{
    public PathCycleManager manager;

    [Tooltip("Which set this tile belongs to")]
    public int setIndex;

    [Tooltip("Index of this tile inside the set")]
    public int pathIndex;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (manager.counterValue == pathIndex)
        {
            manager.PauseTimer();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        manager.ResumeTimer();
    }
}