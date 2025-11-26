using UnityEngine;

public class Immediately_Destroy : MonoBehaviour
{
    void Update()
    {
       var copy = this;;
       Destroy(copy.gameObject);
    }
}
