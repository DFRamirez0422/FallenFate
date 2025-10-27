// This code is to test the call function of the QTE
// Press T to trigger
using UnityEngine;

public class QTECallTesting : MonoBehaviour
{
    void Update()
    {
        // Press T to call the QTE 
        if (Input.GetKeyDown(KeyCode.T))
        {
            QTEManager.Instance.StartQTE();
        }
    }
}