using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowString : MonoBehaviour
{
    public GameObject Self;
    public GameObject String;

    void OnEnable()
    {
        Invoke(nameof(AttackWithString), 1);
    }

    public void AttackWithString()
    {
        Instantiate (String, transform.position, Quaternion.identity);
        Self.SetActive(false);

    }
}
