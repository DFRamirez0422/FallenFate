using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    public string locationName;
    public LocationUI locationUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            locationUI.ShowLocation(locationName);
        }
    }
}