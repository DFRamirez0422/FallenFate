using System.Diagnostics;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public bool InCombat;
    public ElenaAI elenaAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elenaAI = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InCombat)
        {
            elenaAI.CombatToggle = true;
        }
        else
        {
            elenaAI.CombatToggle = false;
        }
    }
}
