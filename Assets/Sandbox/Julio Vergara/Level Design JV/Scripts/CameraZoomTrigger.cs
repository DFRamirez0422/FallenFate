using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<PaperMarioCamera>().SetZoom(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<PaperMarioCamera>().SetZoom(false);
        }
    }
}
