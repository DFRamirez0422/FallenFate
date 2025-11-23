using UnityEngine;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header ("Events")]
    [Tooltip ("Called when a note is hit")]
    public OnNoteEventHandler onNoteHit;
    public string eventID = "Drums";


    void Start()
    {
        Koreographer.Instance.RegisterForEvents(eventID, OnKoreographyEvent);
    }

	[System.Serializable]
	public class OnNoteEventHandler : UnityEngine.Events.UnityEvent
	{

	}

    // This method matches the KoreographyEventCallback signature required by RegisterForEvents
    private void OnKoreographyEvent(KoreographyEvent evt)
    {
        OnNoteHit();
    }

    // This method can be called directly or from the Koreography callback
    public void OnNoteHit()
    {
        onNoteHit?.Invoke();
    }
}
