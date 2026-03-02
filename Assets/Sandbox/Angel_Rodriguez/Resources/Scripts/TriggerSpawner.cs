using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public enum TriggerAction
    {
        Spawn,
        Despawn
    }
    
    [System.Serializable]
    public class TriggerObject
    {
        public GameObject targetObject;
        public TriggerAction action;
    }

    [Header("Objects to toggle on player enter")]
    public List<TriggerObject> targetObjects = new List<TriggerObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        for (int i = 0; i < targetObjects.Count; i++)
        {
            bool shouldBeActive = targetObjects[i].action == TriggerAction.Spawn;
            if (targetObjects[i].action == TriggerAction.Spawn)
            {
                targetObjects[i].targetObject.SetActive(shouldBeActive);
            }
            else if (targetObjects[i].action == TriggerAction.Despawn)
            {
                targetObjects[i].targetObject.SetActive(false);
            }
        }
    }
}
