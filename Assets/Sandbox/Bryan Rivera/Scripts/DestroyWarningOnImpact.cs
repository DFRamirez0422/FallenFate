using UnityEngine;

public class DestroyWarningOnImpact : MonoBehaviour
{
    private GameObject warning;

    public void SetWarning(GameObject w)
    {
        warning = w;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (warning != null)
        {
            Destroy(warning);
        }
        Destroy(this); // Optional: remove this script from memory after use
    }
}
