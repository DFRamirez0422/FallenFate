using UnityEngine;

public class NathansGenerator : MonoBehaviour
{
    public GameObject[] enemyList;
    private Activate_Generators generator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator = GetComponent<Activate_Generators>();
    }

    // Update is called once per frame
    void Update()
    {
        if (generator.Activate_Generator == true) 
        {
            Debug.Log("Genrator On");
            for (int i = 0; i < enemyList.Length; i++)
            {
                Debug.Log (enemyList[i]);
                enemyList[i].GetComponent<EnemyHealth>().ChangeHealth(-99);
            }
        }
    }
}
