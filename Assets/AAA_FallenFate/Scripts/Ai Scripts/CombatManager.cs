using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public bool InCombat = false;
    public ElenaAI elenaAI;

    public float CombatTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elenaAI = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (elenaAI != null)
        {
            if (InCombat)
            {
                elenaAI.CombatToggle = true;
                CombatTimer = CombatTimer - Time.deltaTime;
            }
            else
            {
                elenaAI.CombatToggle = false;
            }

            if (CombatTimer < 0)
            {
                InCombat = false;
            }
        }
    }

    public void CombatFuntion()
    {
        InCombat = true;
        CombatTimer = 3;
    }
}
