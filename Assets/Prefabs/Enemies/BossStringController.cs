using System.Collections;
using UnityEngine;

public class BossStringController : MonoBehaviour
{
    //bool alreadyAttacked;
    public int ShadowUptime = 2;
    public int DelayBetweenAttacks = 2;

    public GameObject String1;
    public GameObject String2;
    public GameObject String3;
    public GameObject String4;
    public GameObject String5;
    public GameObject String6;

    public int AmountOfCombos = 3;
    public int ComboCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            StartCoroutine(BeginCombos());
        }
    }

    public IEnumerator BeginCombos()
    {
        for (int i = 0; i < AmountOfCombos; i++)
        {
            StringAttack();
            print("Attack number" + i);
            yield return new WaitForSeconds(ShadowUptime + DelayBetweenAttacks);
        }
        
    }

    //private void ResetAttack() // not used right now.
    //{
    //    alreadyAttacked = false; //Sets alreadyAttacked to false allowing the enemy to attack again 
    //}

    public void StringAttack()
    { 
        int RandomAttack = Random.Range(0, 5);

        if (RandomAttack == 0)
        {
            Combo1();
        }
        if (RandomAttack == 1)
        {
            Combo2();
        }
        if (RandomAttack == 2)
        {
            Combo3();
        }
        if (RandomAttack == 3)
        {
            Combo4();
        }
        if (RandomAttack == 4)
        {
            Combo5();
        }
        if (RandomAttack == 5)
        {
            Combo5();
        }
    }

    //Unused combos
    public void Combo1()
    {
        String1.SetActive(true);
        String2.SetActive(true);
        String3.SetActive(true);
    }

    public void Combo2()
    {
        String4.SetActive(true);
        String5.SetActive(true);
        String6.SetActive(true);
    }

    public void Combo3()
    {
        String1.SetActive(true);
        String3.SetActive(true);
        String5.SetActive(true);
    }

    public void Combo4()
    {
        String2.SetActive(true);
        String4.SetActive(true);
        String6.SetActive(true);
    }

    public void Combo5()
    {
        String1.SetActive(true);
        String2.SetActive(true);
        String5.SetActive(true);
        String6.SetActive(true);
    }
}
