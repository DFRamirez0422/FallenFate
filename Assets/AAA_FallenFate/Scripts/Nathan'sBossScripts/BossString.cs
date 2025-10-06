using UnityEngine;

public class BossString : MonoBehaviour
{
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

    }

    public void StringAttack()
    { 
        int RandomAttack = Random.Range(0, 10);

        if (RandomAttack == 1)
        {
            String1.SetActive(true);
        }
    }


    
}
