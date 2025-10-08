using UnityEngine;

public class BossStringController : MonoBehaviour
{
    bool alreadyAttacked;
    public int timeBetweenAttacks = 2;

    public GameObject String1;
    public GameObject String2;
    public GameObject String3;
    public GameObject String4;
    public GameObject String5;
    public GameObject String6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!alreadyAttacked)
        {
            StringAttack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; //Sets alreadyAttacked to false allowing the enemy to attack again 
    }

    public void StringAttack()
    { 
        int RandomAttack = Random.Range(0, 5);

        if (RandomAttack == 0)
        {
            String1.SetActive(true);
        }
        if (RandomAttack == 1)
        {
            String2.SetActive(true);
        }
        if (RandomAttack == 2)
        {
            String3.SetActive(true);
        }
        if (RandomAttack == 3)
        {
            String4.SetActive(true);
        }
        if (RandomAttack == 4)
        {
            String5.SetActive(true);
        }
        if (RandomAttack == 5)
        {
            String6.SetActive(true);
        }
    }


    
}
