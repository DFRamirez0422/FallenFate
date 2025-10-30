using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowString : MonoBehaviour
{
    //Make Sure to tag the boss with a Boss tag.
    [Header("Make Sure to tag the boss with a Boss tag.")]
    public GameObject Self;
    public GameObject AttackingString;
    private GameObject Boss;
    private BossStringController bossController;

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
        Instantiate(AttackingString, transform.position, Quaternion.identity);
        Self.SetActive(false);
    }
}
