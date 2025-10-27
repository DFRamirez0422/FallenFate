using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowString : MonoBehaviour
{
    //Make Sure to tag the boss with a Boss tag.
    public GameObject Self;
    public GameObject String;
    private GameObject Boss;
    public BossStringController bossController;

    private void Start()
    {
        Boss = GameObject.FindGameObjectWithTag("Boss");
        bossController = Boss.GetComponent<BossStringController>();
        Self.SetActive(false);
    }
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
