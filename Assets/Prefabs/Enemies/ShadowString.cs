using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowString : MonoBehaviour
{
    public GameObject Self;
    public GameObject String;
    public BossStringController bossController;

    void OnEnable()
    {
        Invoke(nameof(AttackWithString), bossController.ShadowUptime);
    }

    public void AttackWithString()
    {
        Instantiate (String, transform.position, Quaternion.identity);
        Self.SetActive(false);

    }
}
