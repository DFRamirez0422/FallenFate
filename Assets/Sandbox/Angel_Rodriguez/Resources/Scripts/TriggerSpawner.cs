using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerSpawner : MonoBehaviour
{
    public enum TriggerAction
    {
        Spawn,
        Despawn
    }

    [System.Serializable] public class ObjectToToggle
    {
        public GameObject targetObject;
        public TriggerAction action;
    }

    bool hasBeenTriggered = false;

    public List<ObjectToToggle> objectsToToggle = new List<ObjectToToggle>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Hitboxs"))
            return;
        if (hasBeenTriggered)
            return; 
        if(collision.CompareTag("Hitboxs"))
        {
            SpawnOrDespawnObjects(objectsToToggle);
            hasBeenTriggered = true;
        }
        

    }

    private void SpawnOrDespawnObjects(List<ObjectToToggle> objectsToToggle)
    {
        for (int i = 0; i < objectsToToggle.Count; i++)
        {
            if (objectsToToggle[i].targetObject != null && objectsToToggle[i].action == TriggerAction.Spawn)
            {
                objectsToToggle[i].targetObject.SetActive(true);
            }
            else if (objectsToToggle[i].targetObject != null && objectsToToggle[i].action == TriggerAction.Despawn)
            {
                objectsToToggle[i].targetObject.SetActive(false);
            }
            else
            {
                return;
            }
        }
    }
}