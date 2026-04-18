using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DelayTriggerSpawner : MonoBehaviour
{
    public enum TriggerAction
    {
        Spawn,
        Despawn
    }

    [Header("Objects to toggle on player enter")]
    [Tooltip("Objects affected when the player enters this trigger.")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("Action to apply on enter")]
    [Tooltip("Spawn turns the objects on. Despawn turns them off.")]
    public TriggerAction action = TriggerAction.Spawn;

    [Header("Delay Before Action")]
    [Tooltip("How long to wait after the player enters the trigger before applying the action.")]
    public float actionDelay = 0.35f;

    [Header("Trigger Behavior")]
    [Tooltip("If true, the trigger only works once.")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        hasTriggered = true;
        StartCoroutine(ApplyActionAfterDelay());
    }

    private IEnumerator ApplyActionAfterDelay()
    {
        if (actionDelay > 0f)
        {
            yield return new WaitForSeconds(actionDelay);
        }

        bool shouldBeActive = action == TriggerAction.Spawn;

        for (int i = 0; i < targetObjects.Count; i++)
        {
            if (targetObjects[i] != null)
            {
                targetObjects[i].SetActive(shouldBeActive);
            }
        }
    }
}