using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerObjectSpawner : MonoBehaviour
{
    public enum TriggerAction
    {
        Spawn,
        Despawn
    }

    [Header("Objects to toggle on player enter")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("Action to apply on enter")]
    public TriggerAction action = TriggerAction.Spawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
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
