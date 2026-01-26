using UnityEngine;

public class Sword : MonoBehaviour
{
    [HideInInspector]
    public bool disarmed;

    public GameObject swordOBJ;

    private void Update()
    {
        if (disarmed)
        {
            swordOBJ.SetActive(false); // Can be replaced with whatever attacks to disable it. If a script maybe just disable the entire script.
            Invoke(nameof(Rearm), 2);
        }
    }

    private void Rearm()
    {
        swordOBJ.SetActive(true);  // Can be replaced with whatever attacks to enable it. 
    }
}
