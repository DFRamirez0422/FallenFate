using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
    public static DialogueHistoryTracker Instance;
    private readonly List<ActorSO> m_SpokenNPCs = new List<ActorSO>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RecordNPC(ActorSO actorSO)
    {
        m_SpokenNPCs.Add(actorSO);
        Debug.Log("Just spoke to " + actorSO.m_ActorName);
    }

    public bool HasSpokenWith(ActorSO actorSO)
    {
        return m_SpokenNPCs.Contains(actorSO);
    }
}
