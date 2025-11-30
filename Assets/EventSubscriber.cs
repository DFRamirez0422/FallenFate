using UnityEngine;
using SonicBloom.Koreo;

public class EventSubscriber : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Koreographer.Instance.RegisterForEvents("TestEventID", FireEventDebugLog);
    
    }

    void FireEventDebugLog(KoreographyEvent koreoEvent)
    {
        Debug.Log("Event fired: " + koreoEvent.GetTextValue());
    }
}
