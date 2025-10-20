using UnityEngine;

public class PlayerHealthN : MonoBehaviour
{
    public int Hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hp = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        Hp = Hp - 10;
        Debug.Log("Took Damage");
    }
}
