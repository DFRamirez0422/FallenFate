using UnityEditor.Search;
using UnityEngine;

public class ShadowString : MonoBehaviour
{
    public GameObject String;

    void OnEnable()
    {
        AttackWithString();
    }

    public void AttackWithString()
    {
        Instantiate (String, transform.position, Quaternion.identity);
    }
}
