using UnityEngine;

public class Melee : MonoBehaviour
{
    public float lifeTimer = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer = lifeTimer - Time.deltaTime;

        if (lifeTimer <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
