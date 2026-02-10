using UnityEngine;

public class Disarm : MonoBehaviour
{
    [HideInInspector]
    public bool disarmed;

    [HideInInspector]
    public GameObject swordOBJ;

    private void Update()
    {
        if (disarmed)
        {
            swordOBJ.SetActive(false); // Can be replaced with whatever attacks to disable it. If a script maybe just disable the entire script.
        }
    }

    public void Rearm()
    {
        Debug.Log("Rearming");
        disarmed = false;
        swordOBJ.SetActive(true);  // Can be replaced with whatever attacks to enable it. 
    }
}
